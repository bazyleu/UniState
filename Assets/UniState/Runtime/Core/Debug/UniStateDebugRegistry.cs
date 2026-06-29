#if UNISTATE_DEBUG_TREE

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace UniState
{
    internal static class UniStateDebugRegistry
    {
        private const int MaxTransitionsPerMachine = 256;

        private static readonly object Sync = new();
        private static readonly ConditionalWeakTable<object, DebugObjectId> MachineIds = new();
        private static readonly ConditionalWeakTable<object, DebugObjectId> StateIds = new();
        private static readonly Dictionary<int, MachineRecord> Machines = new();
        private static readonly AsyncLocal<ExecutionScope> CurrentScope = new();

        private static int _nextMachineId;
        private static int _nextStateId;
        private static long _sequence;

        public static void RegisterMachine(StateMachine machine, ITypeResolver resolver, int maxHistorySize) =>
            Guard(() =>
            {
                lock (Sync)
                {
                    var record = EnsureMachine(machine);
                    record.ResolverType = GetTypeName(resolver?.GetType());
                    record.MaxHistorySize = maxHistorySize;

                    if (record.Status != DebugMachineStatus.Running)
                    {
                        record.Status = DebugMachineStatus.Registered;
                    }
                }
            });

        public static void MachineExecuteRequested(StateMachine machine, StateTransitionInfo initialTransition) =>
            Guard(() =>
            {
                lock (Sync)
                {
                    var record = EnsureMachine(machine);
                    record.Status = DebugMachineStatus.Running;
                    record.ParentStateId = CurrentScope.Value?.StateId;
                    record.ActiveStateId = null;
                    record.LastError = null;
                    record.History.Clear();
                    record.States.Clear();
                    record.RecentTransitions.Clear();
                    _sequence++;
                }
            });

        public static void MachineAlreadyExecuting(StateMachine machine, Exception exception) =>
            Guard(() =>
            {
                lock (Sync)
                {
                    var record = EnsureMachine(machine);
                    var error = BuildError("AlreadyExecuting", exception, null, DebugStatePhase.None);
                    record.LastError = error;
                }
            });

        public static void MachineFinished(StateMachine machine, DebugMachineStatus status) =>
            Guard(() =>
            {
                lock (Sync)
                {
                    var record = EnsureMachine(machine);

                    if (status == DebugMachineStatus.Failed ||
                        status == DebugMachineStatus.Cancelled ||
                        record.Status == DebugMachineStatus.Running)
                    {
                        record.Status = status;
                    }

                    if (status != DebugMachineStatus.Running)
                    {
                        record.ActiveStateId = null;
                    }

                    _sequence++;
                }
            });

        public static void StateCreated(
            StateMachine machine,
            IExecutableState state,
            Type declaredType,
            Type runtimeType,
            Type payloadType,
            DebugStateRole role) =>
            Guard(() =>
            {
                if (machine == null || state == null)
                {
                    return;
                }

                lock (Sync)
                {
                    var machineRecord = EnsureMachine(machine);
                    EnsureState(machineRecord, state, declaredType, runtimeType, payloadType, role);
                }
            });

        public static void CompositeSubStatesCreated(
            StateMachine machine,
            IExecutableState compositeState,
            Type payloadType,
            IReadOnlyList<IExecutableState> subStates) =>
            Guard(() =>
            {
                if (machine == null || compositeState == null || subStates == null)
                {
                    return;
                }

                lock (Sync)
                {
                    var machineRecord = EnsureMachine(machine);
                    var parentRecord = EnsureState(
                        machineRecord,
                        compositeState,
                        compositeState.GetType(),
                        compositeState.GetType(),
                        payloadType,
                        DebugStateRole.CompositeState);

                    parentRecord.Role = DebugStateRole.CompositeState;
                    parentRecord.SubStateIds.Clear();

                    for (var i = 0; i < subStates.Count; i++)
                    {
                        var subState = subStates[i];
                        if (subState == null)
                        {
                            continue;
                        }

                        var childRecord = EnsureState(
                            machineRecord,
                            subState,
                            subState.GetType(),
                            subState.GetType(),
                            payloadType,
                            DebugStateRole.SubState);

                        childRecord.Role = DebugStateRole.SubState;
                        childRecord.ParentCompositeStateId = parentRecord.Id;
                        parentRecord.SubStateIds.Add(childRecord.Id);
                    }
                }
            });

        public static IDisposable BeginStatePhase(StateMachine machine, IExecutableState state, DebugStatePhase phase) =>
            Guard(() =>
            {
                if (machine == null || state == null)
                {
                    return DebugScope.Empty;
                }

                lock (Sync)
                {
                    var machineRecord = EnsureMachine(machine);
                    return BeginStatePhase(machineRecord, state, phase);
                }
            }, DebugScope.Empty);

        public static IDisposable BeginStatePhase(IExecutableState state, DebugStatePhase phase) =>
            Guard(() =>
            {
                if (state == null)
                {
                    return DebugScope.Empty;
                }

                lock (Sync)
                {
                    var machineRecord = FindMachineByState(state);
                    return machineRecord == null ? DebugScope.Empty : BeginStatePhase(machineRecord, state, phase);
                }
            }, DebugScope.Empty);

        public static void EndStatePhase(IExecutableState state, DebugStatePhase phase) =>
            Guard(() =>
            {
                if (state == null)
                {
                    return;
                }

                lock (Sync)
                {
                    var stateRecord = FindStateRecord(state);
                    if (stateRecord == null)
                    {
                        return;
                    }

                    if (stateRecord.Phase == phase)
                    {
                        stateRecord.Phase = DebugStatePhase.None;
                    }

                    if (phase == DebugStatePhase.Exit)
                    {
                        stateRecord.Status = DebugStateStatus.Completed;
                    }

                    if (phase == DebugStatePhase.Dispose)
                    {
                        stateRecord.Status = DebugStateStatus.Disposed;
                    }
                }
            });

        public static void StateCancelled(IExecutableState state) =>
            Guard(() =>
            {
                if (state == null)
                {
                    return;
                }

                lock (Sync)
                {
                    var stateRecord = FindStateRecord(state);
                    if (stateRecord == null)
                    {
                        return;
                    }

                    stateRecord.Status = DebugStateStatus.Cancelled;
                    stateRecord.Phase = DebugStatePhase.None;
                }
            });

        public static void StateChanged(StateMachine machine, StateMachineStateChangedData changeData) =>
            Guard(() =>
            {
                if (machine == null || changeData == null)
                {
                    return;
                }

                lock (Sync)
                {
                    var machineRecord = EnsureMachine(machine);
                    var previousState = changeData.PreviousState == null
                        ? null
                        : EnsureState(machineRecord, changeData.PreviousState);
                    var currentState = changeData.CurrentState == null
                        ? null
                        : EnsureState(machineRecord, changeData.CurrentState);

                    if (previousState != null &&
                        changeData.ChangeType != StateMachineStateChangeType.Started)
                    {
                        previousState.Status = DebugStateStatus.Disposed;
                        previousState.Phase = DebugStatePhase.None;
                    }

                    if (currentState != null)
                    {
                        currentState.Status = DebugStateStatus.Active;
                        currentState.Phase = DebugStatePhase.None;
                        machineRecord.ActiveStateId = currentState.Id;
                    }
                    else
                    {
                        machineRecord.ActiveStateId = null;
                    }

                    var transition = BuildTransition(changeData, previousState, currentState);
                    AddTransition(machineRecord, transition);

                    if (previousState != null)
                    {
                        previousState.LastTransition = transition;
                    }

                    if (currentState != null)
                    {
                        currentState.LastTransition = transition;
                    }
                }
            });

        public static void HistoryPushed(StateMachine machine, IExecutableState state, StateTransitionInfo transitionInfo) =>
            Guard(() =>
            {
                if (machine == null || state == null || transitionInfo == null)
                {
                    return;
                }

                lock (Sync)
                {
                    var machineRecord = EnsureMachine(machine);
                    if (machineRecord.MaxHistorySize <= 0)
                    {
                        return;
                    }

                    var stateRecord = EnsureState(machineRecord, state);
                    machineRecord.History.Add(ToStateRef(stateRecord));

                    while (machineRecord.History.Count > machineRecord.MaxHistorySize)
                    {
                        machineRecord.History.RemoveAt(0);
                    }
                }
            });

        public static void HistoryPopped(StateMachine machine) =>
            Guard(() =>
            {
                if (machine == null)
                {
                    return;
                }

                lock (Sync)
                {
                    var machineRecord = EnsureMachine(machine);
                    if (machineRecord.History.Count > 0)
                    {
                        machineRecord.History.RemoveAt(machineRecord.History.Count - 1);
                    }
                }
            });

        public static void HistoryMissed(StateMachine machine, Type goBackToType) =>
            Guard(() =>
            {
                if (machine == null)
                {
                    return;
                }

                lock (Sync)
                {
                    EnsureMachine(machine);
                    _sequence++;
                }
            });

        public static void Error(StateMachine machine, StateMachineErrorData errorData) =>
            Guard(() =>
            {
                if (machine == null || errorData == null)
                {
                    return;
                }

                lock (Sync)
                {
                    var machineRecord = EnsureMachine(machine);
                    var stateRecord = errorData.State == null ? null : EnsureState(machineRecord, errorData.State);
                    ApplyError(machineRecord, stateRecord, errorData.ErrorType.ToString(), errorData.Exception);
                }
            });

        public static void Error(IExecutableState state, StateMachineErrorType errorType, Exception exception) =>
            Guard(() =>
            {
                if (state == null || exception == null)
                {
                    return;
                }

                lock (Sync)
                {
                    var machineRecord = FindMachineByState(state);
                    if (machineRecord == null)
                    {
                        return;
                    }

                    var stateRecord = EnsureState(machineRecord, state);
                    ApplyError(machineRecord, stateRecord, errorType.ToString(), exception);
                }
            });

        public static void NoSubStates(Exception exception) =>
            Guard(() =>
            {
                var scope = CurrentScope.Value;
                if (scope == null || exception == null)
                {
                    return;
                }

                lock (Sync)
                {
                    if (!Machines.TryGetValue(scope.MachineId, out var machineRecord) ||
                        !machineRecord.States.TryGetValue(scope.StateId, out var stateRecord))
                    {
                        return;
                    }

                    ApplyError(machineRecord, stateRecord, StateMachineErrorType.StateExecuting.ToString(), exception);
                }
            });

        public static void SubStateCompleted(IExecutableState state, StateTransitionInfo transitionInfo) =>
            Guard(() =>
            {
                if (state == null)
                {
                    return;
                }

                lock (Sync)
                {
                    var stateRecord = FindStateRecord(state);
                    if (stateRecord == null)
                    {
                        return;
                    }

                    stateRecord.Status = DebugStateStatus.Completed;
                    stateRecord.Phase = DebugStatePhase.None;
                }
            });

        public static void SubStatesCancelled<TState>(IReadOnlyList<TState> states, int winnerIndex)
            where TState : IExecutableState =>
            Guard(() =>
            {
                if (states == null)
                {
                    return;
                }

                lock (Sync)
                {
                    for (var i = 0; i < states.Count; i++)
                    {
                        if (i == winnerIndex)
                        {
                            continue;
                        }

                        var stateRecord = FindStateRecord(states[i]);
                        if (stateRecord == null || stateRecord.Status == DebugStateStatus.Disposed)
                        {
                            continue;
                        }

                        stateRecord.Status = DebugStateStatus.Cancelled;
                        stateRecord.Phase = DebugStatePhase.None;
                    }
                }
            });

        public static UniStateDebugSnapshot GetSnapshot() =>
            Guard(() =>
            {
                lock (Sync)
                {
                    var machines = new List<DebugMachineNode>(Machines.Count);

                    foreach (var pair in Machines)
                    {
                        machines.Add(CloneMachine(pair.Value));
                    }

                    machines.Sort((left, right) => left.Id.CompareTo(right.Id));

                    return new UniStateDebugSnapshot
                    {
                        Sequence = _sequence,
                        Machines = machines
                    };
                }
            }, UniStateDebugSnapshot.Empty());

        public static void ClearCompleted() =>
            Guard(() =>
            {
                lock (Sync)
                {
                    var ids = new List<int>();

                    foreach (var pair in Machines)
                    {
                        if (pair.Value.Status != DebugMachineStatus.Running)
                        {
                            ids.Add(pair.Key);
                        }
                    }

                    for (var i = 0; i < ids.Count; i++)
                    {
                        Machines.Remove(ids[i]);
                    }

                    _sequence++;
                }
            });

        public static void ClearAll() =>
            Guard(() =>
            {
                lock (Sync)
                {
                    Machines.Clear();
                    _sequence = 0;
                    CurrentScope.Value = null;
                }
            });

        private static IDisposable BeginStatePhase(
            MachineRecord machineRecord,
            IExecutableState state,
            DebugStatePhase phase)
        {
            var stateRecord = EnsureState(machineRecord, state);
            stateRecord.Phase = phase;

            if (stateRecord.Status != DebugStateStatus.Failed)
            {
                stateRecord.Status = DebugStateStatus.Active;
            }

            var previousScope = CurrentScope.Value;
            CurrentScope.Value = new ExecutionScope(machineRecord.Id, stateRecord.Id, phase, previousScope);

            return new DebugScope(previousScope);
        }

        private static MachineRecord EnsureMachine(StateMachine machine)
        {
            var id = GetId(MachineIds, machine, ref _nextMachineId);

            if (!Machines.TryGetValue(id, out var record))
            {
                record = new MachineRecord
                {
                    Id = id,
                    RuntimeType = GetTypeName(machine.GetType()),
                    Status = DebugMachineStatus.Registered,
                    WeakMachine = new WeakReference<StateMachine>(machine),
                    MaxHistorySize = 15
                };
                Machines[id] = record;
            }

            return record;
        }

        private static StateRecord EnsureState(
            MachineRecord machine,
            IExecutableState state,
            Type declaredType = null,
            Type runtimeType = null,
            Type payloadType = null,
            DebugStateRole role = DebugStateRole.State)
        {
            var id = GetId(StateIds, state, ref _nextStateId);

            if (!machine.States.TryGetValue(id, out var record))
            {
                var actualRuntimeType = runtimeType ?? state.GetType();
                record = new StateRecord
                {
                    Id = id,
                    DeclaredType = GetTypeName(declaredType ?? actualRuntimeType),
                    RuntimeType = GetTypeName(actualRuntimeType),
                    PayloadType = GetTypeName(payloadType),
                    Role = role,
                    Status = DebugStateStatus.Created,
                    Phase = DebugStatePhase.None
                };

                machine.States[id] = record;
            }
            else
            {
                if (declaredType != null)
                {
                    record.DeclaredType = GetTypeName(declaredType);
                }

                if (runtimeType != null)
                {
                    record.RuntimeType = GetTypeName(runtimeType);
                }

                if (payloadType != null)
                {
                    record.PayloadType = GetTypeName(payloadType);
                }

                if (role != DebugStateRole.State || record.Role == DebugStateRole.State)
                {
                    record.Role = role;
                }
            }

            return record;
        }

        private static int GetId(ConditionalWeakTable<object, DebugObjectId> table, object instance, ref int nextId)
        {
            if (table.TryGetValue(instance, out var identity))
            {
                return identity.Id;
            }

            identity = new DebugObjectId(++nextId);
            table.Add(instance, identity);

            return identity.Id;
        }

        private static MachineRecord FindMachineByState(IExecutableState state)
        {
            var stateId = GetExistingId(StateIds, state);
            if (!stateId.HasValue)
            {
                return null;
            }

            foreach (var pair in Machines)
            {
                if (pair.Value.States.ContainsKey(stateId.Value))
                {
                    return pair.Value;
                }
            }

            return null;
        }

        private static StateRecord FindStateRecord(IExecutableState state)
        {
            var stateId = GetExistingId(StateIds, state);
            if (!stateId.HasValue)
            {
                return null;
            }

            foreach (var pair in Machines)
            {
                if (pair.Value.States.TryGetValue(stateId.Value, out var stateRecord))
                {
                    return stateRecord;
                }
            }

            return null;
        }

        private static int? GetExistingId(ConditionalWeakTable<object, DebugObjectId> table, object instance)
        {
            return table.TryGetValue(instance, out var identity) ? identity.Id : (int?)null;
        }

        private static void ApplyError(
            MachineRecord machineRecord,
            StateRecord stateRecord,
            string errorType,
            Exception exception)
        {
            var error = BuildError(errorType, exception, stateRecord, stateRecord?.Phase ?? DebugStatePhase.None);
            machineRecord.LastError = error;

            if (stateRecord != null)
            {
                stateRecord.LastError = error;
                stateRecord.Status = DebugStateStatus.Failed;
                stateRecord.Phase = DebugStatePhase.None;
            }

            if (errorType == StateMachineErrorType.StateMachineFail.ToString())
            {
                machineRecord.Status = DebugMachineStatus.Failed;
            }
        }

        private static DebugErrorInfo BuildError(
            string errorType,
            Exception exception,
            StateRecord stateRecord,
            DebugStatePhase phase)
        {
            return new DebugErrorInfo
            {
                Sequence = ++_sequence,
                ErrorType = errorType,
                ExceptionType = GetTypeName(exception.GetType()),
                Message = exception.Message,
                StateId = stateRecord?.Id,
                StateType = stateRecord?.RuntimeType,
                Phase = phase
            };
        }

        private static DebugTransitionEvent BuildTransition(
            StateMachineStateChangedData changeData,
            StateRecord previousState,
            StateRecord currentState)
        {
            var requestedTransition = changeData.RequestedTransition;

            return new DebugTransitionEvent
            {
                Sequence = ++_sequence,
                Transition = requestedTransition?.Transition ?? TransitionType.State,
                FromStateId = previousState?.Id,
                ToStateId = currentState?.Id,
                FromStateType = previousState?.RuntimeType,
                ToStateType = currentState?.RuntimeType,
                GoBackToType = GetTypeName(requestedTransition?.GoBackToType)
            };
        }

        private static void AddTransition(MachineRecord machineRecord, DebugTransitionEvent transition)
        {
            machineRecord.RecentTransitions.Add(transition);

            while (machineRecord.RecentTransitions.Count > MaxTransitionsPerMachine)
            {
                machineRecord.RecentTransitions.RemoveAt(0);
            }
        }

        private static DebugMachineNode CloneMachine(MachineRecord record)
        {
            var history = new List<DebugStateRef>(record.History.Count);
            for (var i = 0; i < record.History.Count; i++)
            {
                history.Add(CloneStateRef(record.History[i]));
            }

            var states = new List<DebugStateNode>(record.States.Count);
            foreach (var pair in record.States)
            {
                states.Add(CloneState(pair.Value));
            }

            states.Sort((left, right) => left.Id.CompareTo(right.Id));

            var transitions = new List<DebugTransitionEvent>(record.RecentTransitions.Count);
            for (var i = 0; i < record.RecentTransitions.Count; i++)
            {
                transitions.Add(CloneTransition(record.RecentTransitions[i]));
            }

            return new DebugMachineNode
            {
                Id = record.Id,
                RuntimeType = record.RuntimeType,
                ResolverType = record.ResolverType,
                Status = record.Status,
                ParentStateId = record.ParentStateId,
                ActiveStateId = record.ActiveStateId,
                History = history,
                States = states,
                RecentTransitions = transitions,
                LastError = CloneError(record.LastError)
            };
        }

        private static DebugStateNode CloneState(StateRecord record) =>
            new DebugStateNode
            {
                Id = record.Id,
                DeclaredType = record.DeclaredType,
                RuntimeType = record.RuntimeType,
                PayloadType = record.PayloadType,
                Role = record.Role,
                Status = record.Status,
                Phase = record.Phase,
                ParentCompositeStateId = record.ParentCompositeStateId,
                SubStateIds = new List<int>(record.SubStateIds),
                LastTransition = CloneTransition(record.LastTransition),
                LastError = CloneError(record.LastError)
            };

        private static DebugTransitionEvent CloneTransition(DebugTransitionEvent transition)
        {
            if (transition == null)
            {
                return null;
            }

            return new DebugTransitionEvent
            {
                Sequence = transition.Sequence,
                Transition = transition.Transition,
                FromStateId = transition.FromStateId,
                ToStateId = transition.ToStateId,
                FromStateType = transition.FromStateType,
                ToStateType = transition.ToStateType,
                GoBackToType = transition.GoBackToType
            };
        }

        private static DebugErrorInfo CloneError(DebugErrorInfo error)
        {
            if (error == null)
            {
                return null;
            }

            return new DebugErrorInfo
            {
                Sequence = error.Sequence,
                ErrorType = error.ErrorType,
                ExceptionType = error.ExceptionType,
                Message = error.Message,
                StateId = error.StateId,
                StateType = error.StateType,
                Phase = error.Phase
            };
        }

        private static DebugStateRef CloneStateRef(DebugStateRef state) =>
            new DebugStateRef
            {
                Id = state.Id,
                DeclaredType = state.DeclaredType,
                RuntimeType = state.RuntimeType
            };

        private static DebugStateRef ToStateRef(StateRecord state) =>
            new DebugStateRef
            {
                Id = state.Id,
                DeclaredType = state.DeclaredType,
                RuntimeType = state.RuntimeType
            };

        private static string GetTypeName(Type type) => type == null ? null : type.FullName;

        private static void Guard(Action action)
        {
            try
            {
                action();
            }
            catch
            {
            }
        }

        private static T Guard<T>(Func<T> action, T fallback)
        {
            try
            {
                return action();
            }
            catch
            {
                return fallback;
            }
        }

        private sealed class DebugObjectId
        {
            public int Id { get; }

            public DebugObjectId(int id)
            {
                Id = id;
            }
        }

        private sealed class MachineRecord
        {
            public int Id;
            public string RuntimeType;
            public string ResolverType;
            public DebugMachineStatus Status;
            public int MaxHistorySize;
            public int? ParentStateId;
            public int? ActiveStateId;
            public readonly List<DebugStateRef> History = new();
            public readonly Dictionary<int, StateRecord> States = new();
            public readonly List<DebugTransitionEvent> RecentTransitions = new();
            public DebugErrorInfo LastError;
            public WeakReference<StateMachine> WeakMachine;
        }

        private sealed class StateRecord
        {
            public int Id;
            public string DeclaredType;
            public string RuntimeType;
            public string PayloadType;
            public DebugStateRole Role;
            public DebugStateStatus Status;
            public DebugStatePhase Phase;
            public int? ParentCompositeStateId;
            public readonly List<int> SubStateIds = new();
            public DebugTransitionEvent LastTransition;
            public DebugErrorInfo LastError;
        }

        private sealed class ExecutionScope
        {
            public int MachineId { get; }
            public int StateId { get; }
            public DebugStatePhase Phase { get; }
            public ExecutionScope Previous { get; }

            public ExecutionScope(int machineId, int stateId, DebugStatePhase phase, ExecutionScope previous)
            {
                MachineId = machineId;
                StateId = stateId;
                Phase = phase;
                Previous = previous;
            }
        }

        private sealed class DebugScope : IDisposable
        {
            public static readonly DebugScope Empty = new(null, true);

            private readonly ExecutionScope _previousScope;
            private readonly bool _empty;
            private bool _disposed;

            public DebugScope(ExecutionScope previousScope, bool empty = false)
            {
                _previousScope = previousScope;
                _empty = empty;
            }

            public void Dispose()
            {
                if (_disposed || _empty)
                {
                    return;
                }

                _disposed = true;
                CurrentScope.Value = _previousScope;
            }
        }
    }
}

#endif

using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState
{
    public class StateMachine : IStateMachine
    {
        private LimitedStack<StateTransitionInfo> _history;
        private IStateTransitionFactory _transitionFactory;

        public bool IsExecuting => _isExecuting;
        protected virtual int MaxHistorySize => 15;

        private bool _isExecuting = false;

        public virtual void SetResolver(ITypeResolver resolver)
        {
            _transitionFactory = new StateTransitionFactory(resolver, this);
#if UNISTATE_DEBUG_TREE
            UniStateDebugRegistry.RegisterMachine(this, resolver, MaxHistorySize);
#endif
        }

        public virtual async UniTask Execute<TState>(CancellationToken token) where TState : class, IState<EmptyPayload>
        {
            await ExecuteInternal(_transitionFactory.CreateStateTransition<TState>(), token);
        }

        public virtual async UniTask Execute<TState, TPayload>(TPayload payload, CancellationToken token)
            where TState : class, IState<TPayload>
        {
            await ExecuteInternal(_transitionFactory.CreateStateTransition<TState, TPayload>(payload), token);
        }

        protected virtual void HandleError(StateMachineErrorData errorData)
        {
            UnityEngine.Debug.LogError(errorData.Exception);
        }

        protected virtual StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory) =>
            transitionFactory.CreateBackTransition();

        protected virtual void HandleStateChanged(StateMachineStateChangedData changeData)
        {
        }

        private void Initialize()
        {
            _history = new LimitedStack<StateTransitionInfo>(MaxHistorySize);
            _isExecuting = true;
        }

        private async UniTask ExecuteInternal(StateTransitionInfo initialTransition, CancellationToken token)
        {
            if (_isExecuting)
            {
                var exception = new AlreadyExecutingException();
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.MachineAlreadyExecuting(this, exception);
#endif
                throw exception;
            }

#if UNISTATE_DEBUG_TREE
            UniStateDebugRegistry.MachineExecuteRequested(this, initialTransition);
#endif

            Initialize();
#if UNISTATE_DEBUG_TREE
            var debugFinishStatus = DebugMachineStatus.Completed;
#endif

            var activeStateMetadata = new StateWithMetadata();
            var nextStateMetadata = new StateWithMetadata();
            StateTransitionInfo transitionInfo = null;

            try
            {
                activeStateMetadata.BuildState(initialTransition, initialTransition.StateBehaviourData);
                ProcessStateChanged(new StateMachineStateChangedData(
                    null,
                    activeStateMetadata.State,
                    null,
                    activeStateMetadata.TransitionInfo,
                    initialTransition,
                    StateMachineStateChangeType.Started));

                await InitializeSafe(activeStateMetadata.State, token);

                transitionInfo = await ExecuteSafe(activeStateMetadata.State, token);

                ProcessTransitionInfo(transitionInfo, activeStateMetadata, nextStateMetadata);

                while (!nextStateMetadata.IsEmpty && !token.IsCancellationRequested)
                {
                    var previousState = activeStateMetadata.State;
                    var previousTransition = activeStateMetadata.TransitionInfo;

                    if (nextStateMetadata.BehaviourData.InitializeOnStateTransition)
                    {
                        await InitializeSafe(nextStateMetadata.State, token);
                        await ExitAndDisposeSafe(activeStateMetadata, token);
                    }
                    else
                    {
                        await ExitAndDisposeSafe(activeStateMetadata, token);
                        await InitializeSafe(nextStateMetadata.State, token);
                    }

                    activeStateMetadata.CopyData(nextStateMetadata);
                    ProcessStateChanged(new StateMachineStateChangedData(
                        previousState,
                        activeStateMetadata.State,
                        previousTransition,
                        activeStateMetadata.TransitionInfo,
                        transitionInfo,
                        StateMachineStateChangeType.Changed));

                    transitionInfo = await ExecuteSafe(activeStateMetadata.State, token);

                    ProcessTransitionInfo(transitionInfo, activeStateMetadata, nextStateMetadata);
                }

                var exitedState = activeStateMetadata.State;
                var exitedTransition = activeStateMetadata.TransitionInfo;

                await ExitAndDisposeSafe(activeStateMetadata, token);
                ProcessStateChanged(new StateMachineStateChangedData(
                    exitedState,
                    null,
                    exitedTransition,
                    null,
                    transitionInfo,
                    StateMachineStateChangeType.Exited));
                activeStateMetadata.Clear();
            }
            catch (OperationCanceledException)
            {
#if UNISTATE_DEBUG_TREE
                debugFinishStatus = DebugMachineStatus.Cancelled;
#endif
                throw;
            }
            catch (Exception e)
            {
#if UNISTATE_DEBUG_TREE
                debugFinishStatus = DebugMachineStatus.Failed;
#endif
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateMachineFail));
            }
            finally
            {
                Exception disposeException = null;

                try
                {
                    DisposeSafe(nextStateMetadata);
                }
                catch (Exception e)
                {
                    disposeException ??= e;
                }

                nextStateMetadata.Clear();

                try
                {
                    DisposeSafe(activeStateMetadata);
                }
                catch (Exception e)
                {
                    disposeException ??= e;
                }

                activeStateMetadata.Clear();

                _isExecuting = false;

                if (disposeException != null)
                {
#if UNISTATE_DEBUG_TREE
                    debugFinishStatus = DebugMachineStatus.Failed;
                    UniStateDebugRegistry.MachineFinished(this, debugFinishStatus);
#endif
                    ExceptionDispatchInfo.Capture(disposeException).Throw();
                }

#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.MachineFinished(this, debugFinishStatus);
#endif
            }
        }

        private void ProcessTransitionInfo(StateTransitionInfo nextTransition,
            StateWithMetadata previousStateMetadata,
            StateWithMetadata stateWithMetadata)
        {
            var previousTransition = previousStateMetadata.TransitionInfo;
            stateWithMetadata.Clear();

            if (nextTransition.Transition == TransitionType.Exit)
            {
                return;
            }

            var transitionToState = nextTransition.Transition == TransitionType.State;

            var item = transitionToState ? nextTransition : GetInfoFromHistory(nextTransition);

            if (transitionToState && previousTransition.CanBeAddedToHistory())
            {
                _history.Push(previousTransition);
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.HistoryPushed(this, previousStateMetadata.State, previousTransition);
#endif
            }

            if (item != null)
            {
                stateWithMetadata.BuildState(item, item.StateBehaviourData);
            }
        }

        private StateTransitionInfo GetInfoFromHistory(StateTransitionInfo nextTransition)
        {
            if (nextTransition.GoBackToType == null)
            {
                var info = _history.Pop();
#if UNISTATE_DEBUG_TREE
                if (info == null)
                {
                    UniStateDebugRegistry.HistoryMissed(this, null);
                }
                else
                {
                    UniStateDebugRegistry.HistoryPopped(this);
                }
#endif
                return info;
            }

            while (_history.Count() > 0)
            {
                var info = _history.Pop();
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.HistoryPopped(this);
#endif
                if (nextTransition.GoBackToType == info.Creator?.StateType)
                {
                    return info;
                }
            }

#if UNISTATE_DEBUG_TREE
            UniStateDebugRegistry.HistoryMissed(this, nextTransition.GoBackToType);
#endif
            return null;
        }

        private async UniTask<StateTransitionInfo> ExecuteSafe(IExecutableState state, CancellationToken token)
        {
#if UNISTATE_DEBUG_TREE
            var debugScope = UniStateDebugRegistry.BeginStatePhase(this, state, DebugStatePhase.Execute);
#endif
            try
            {
                token.ThrowIfCancellationRequested();

                var result = await state.Execute(token);
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.EndStatePhase(state, DebugStatePhase.Execute);
#endif
                return result;
            }
            catch (OperationCanceledException)
            {
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.StateCancelled(state);
#endif
                throw;
            }
            catch (Exception e)
            {
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateExecuting, state));
            }
#if UNISTATE_DEBUG_TREE
            finally
            {
                debugScope.Dispose();
            }
#endif

            return BuildRecoveryTransition(_transitionFactory);
        }

        private async UniTask InitializeSafe(IExecutableState state, CancellationToken token)
        {
#if UNISTATE_DEBUG_TREE
            var debugScope = UniStateDebugRegistry.BeginStatePhase(this, state, DebugStatePhase.Initialize);
#endif
            try
            {
                token.ThrowIfCancellationRequested();
                await state.Initialize(token);
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.EndStatePhase(state, DebugStatePhase.Initialize);
#endif
            }
            catch (OperationCanceledException)
            {
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.StateCancelled(state);
#endif
                throw;
            }
            catch (Exception e)
            {
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateInitializing, state));
            }
#if UNISTATE_DEBUG_TREE
            finally
            {
                debugScope.Dispose();
            }
#endif
        }

        private async UniTask ExitAndDisposeSafe(StateWithMetadata metadata, CancellationToken token)
        {
            var state = metadata.State;

#if UNISTATE_DEBUG_TREE
            var debugScope = UniStateDebugRegistry.BeginStatePhase(this, state, DebugStatePhase.Exit);
#endif
            try
            {
                token.ThrowIfCancellationRequested();
                await state.Exit(token);
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.EndStatePhase(state, DebugStatePhase.Exit);
#endif
            }
            catch (OperationCanceledException)
            {
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.StateCancelled(state);
#endif
                throw;
            }
            catch (Exception e)
            {
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateExiting, state));
            }
            finally
            {
#if UNISTATE_DEBUG_TREE
                debugScope.Dispose();
#endif
                DisposeSafe(metadata);
            }
        }

        private void DisposeSafe(StateWithMetadata metadata)
        {
            var state = metadata.State;

#if UNISTATE_DEBUG_TREE
            var debugScope = UniStateDebugRegistry.BeginStatePhase(this, state, DebugStatePhase.Dispose);
#endif
            try
            {
                metadata.Dispose();
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.EndStatePhase(state, DebugStatePhase.Dispose);
#endif
            }
            catch (Exception e)
            {
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateDisposing, state));
            }
#if UNISTATE_DEBUG_TREE
            finally
            {
                debugScope.Dispose();
            }
#endif
        }

        private void ProcessError(StateMachineErrorData errorData)
        {
#if UNISTATE_DEBUG_TREE
            UniStateDebugRegistry.Error(this, errorData);
#endif
            HandleError(errorData);
        }

        private void ProcessStateChanged(StateMachineStateChangedData changeData)
        {
#if UNISTATE_DEBUG_TREE
            UniStateDebugRegistry.StateChanged(this, changeData);
#endif
            HandleStateChanged(changeData);
        }
    }
}

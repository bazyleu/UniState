#if UNISTATE_DEBUG_TREE

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;

namespace UniStateTests.EditMode.Debug
{
    [TestFixture]
    public class UniStateDebugEnabledTests
    {
        [SetUp]
        public void SetUp()
        {
            UniStateDebug.ClearAll();
        }

        [TearDown]
        public void TearDown()
        {
            UniStateDebug.ClearAll();
        }

        [Test]
        public void Execute_WithDebugFlagEnabled_RecordsSnapshotTreeAndJson()
        {
            var stateMachine = new SilentStateMachine();
            stateMachine.SetResolver(new DebugResolver());

            stateMachine.Execute<DebugFirstState>(CancellationToken.None).GetAwaiter().GetResult();

            var snapshot = UniStateDebug.GetSnapshot();
            var machine = snapshot.Machines[0];

            Assert.True(UniStateDebug.IsEnabled);
            Assert.AreEqual(1, snapshot.Machines.Count);
            Assert.AreEqual(DebugMachineStatus.Completed, machine.Status);
            Assert.AreEqual(2, machine.States.Count);
            Assert.AreEqual(1, machine.History.Count);
            Assert.AreEqual(TransitionType.Exit, machine.RecentTransitions[machine.RecentTransitions.Count - 1].Transition);
            Assert.That(UniStateDebug.DumpTree(), Does.Contain("DebugFirstState").And.Contain("Completed"));
            Assert.That(UniStateDebug.DumpJson(false), Does.Contain("\"isEnabled\":true").And.Contain("DebugSecondState"));
        }

        [Test]
        public void Execute_WithCompositeState_RecordsSubStateTree()
        {
            var stateMachine = new SilentStateMachine();
            stateMachine.SetResolver(new DebugResolver());

            stateMachine.Execute<DebugCompositeState>(CancellationToken.None).GetAwaiter().GetResult();

            var machine = UniStateDebug.GetSnapshot().Machines[0];
            var composite = FindState(machine, DebugStateRole.CompositeState);

            Assert.NotNull(composite);
            Assert.AreEqual(2, composite.SubStateIds.Count);
            Assert.AreEqual(2, CountStates(machine, DebugStateRole.SubState));
        }

        [Test]
        public void Execute_WithStateError_RecordsLastError()
        {
            var stateMachine = new SilentStateMachine();
            stateMachine.SetResolver(new DebugResolver());

            stateMachine.Execute<DebugFailingState>(CancellationToken.None).GetAwaiter().GetResult();

            var machine = UniStateDebug.GetSnapshot().Machines[0];

            Assert.NotNull(machine.LastError);
            Assert.AreEqual(StateMachineErrorType.StateExecuting.ToString(), machine.LastError.ErrorType);
            Assert.That(machine.LastError.Message, Does.Contain("Debug failure"));
            Assert.That(UniStateDebug.DumpTree(), Does.Contain("Debug failure"));
        }

        [Test]
        public void Execute_WithNestedMachine_RecordsParentState()
        {
            var resolver = new DebugResolver();
            var stateMachine = new SilentStateMachine();
            stateMachine.SetResolver(resolver);

            stateMachine.Execute<NestedParentState>(CancellationToken.None).GetAwaiter().GetResult();

            var snapshot = UniStateDebug.GetSnapshot();
            DebugMachineNode parentMachine = null;
            DebugMachineNode childMachine = null;

            for (var i = 0; i < snapshot.Machines.Count; i++)
            {
                if (snapshot.Machines[i].ParentStateId.HasValue)
                {
                    childMachine = snapshot.Machines[i];
                }
                else
                {
                    parentMachine = snapshot.Machines[i];
                }
            }

            Assert.NotNull(parentMachine);
            Assert.NotNull(childMachine);

            var parentState = FindState(parentMachine, typeof(NestedParentState));

            Assert.NotNull(parentState);
            Assert.AreEqual(parentState.Id, childMachine.ParentStateId);
        }

        [Test]
        public void Execute_WhenMachineIsAlreadyExecuting_RecordsError()
        {
            BlockingState.CompletionSource = new UniTaskCompletionSource<StateTransitionInfo>();

            var stateMachine = new SilentStateMachine();
            stateMachine.SetResolver(new DebugResolver());

            var runningTask = stateMachine.Execute<BlockingState>(CancellationToken.None);

            Assert.Throws<AlreadyExecutingException>(() =>
                stateMachine.Execute<BlockingState>(CancellationToken.None).GetAwaiter().GetResult());

            BlockingState.CompletionSource.TrySetResult(new StateTransitionInfo { Transition = TransitionType.Exit });
            runningTask.GetAwaiter().GetResult();

            var machine = UniStateDebug.GetSnapshot().Machines[0];

            Assert.NotNull(machine.LastError);
            Assert.AreEqual("AlreadyExecuting", machine.LastError.ErrorType);
            Assert.AreEqual(nameof(AlreadyExecutingException), ShortName(machine.LastError.ExceptionType));
        }

        private static DebugStateNode FindState(DebugMachineNode machine, DebugStateRole role)
        {
            for (var i = 0; i < machine.States.Count; i++)
            {
                if (machine.States[i].Role == role)
                {
                    return machine.States[i];
                }
            }

            return null;
        }

        private static DebugStateNode FindState(DebugMachineNode machine, Type runtimeType)
        {
            for (var i = 0; i < machine.States.Count; i++)
            {
                if (machine.States[i].RuntimeType == runtimeType.FullName)
                {
                    return machine.States[i];
                }
            }

            return null;
        }

        private static int CountStates(DebugMachineNode machine, DebugStateRole role)
        {
            var count = 0;

            for (var i = 0; i < machine.States.Count; i++)
            {
                if (machine.States[i].Role == role)
                {
                    count++;
                }
            }

            return count;
        }

        private static string ShortName(string typeName)
        {
            var index = typeName.LastIndexOf('.');
            return index >= 0 ? typeName.Substring(index + 1) : typeName;
        }

        private sealed class SilentStateMachine : StateMachine
        {
            protected override void HandleError(StateMachineErrorData errorData)
            {
            }
        }

        private sealed class DebugResolver : ITypeResolver
        {
            public object Resolve(Type type)
            {
                if (type == typeof(DebugFirstState))
                {
                    return new DebugFirstState();
                }

                if (type == typeof(DebugSecondState))
                {
                    return new DebugSecondState();
                }

                if (type == typeof(DebugFailingState))
                {
                    return new DebugFailingState();
                }

                if (type == typeof(DebugCompositeState))
                {
                    return new DebugCompositeState();
                }

                if (type == typeof(NestedParentState))
                {
                    return new NestedParentState(this);
                }

                if (type == typeof(NestedChildState))
                {
                    return new NestedChildState();
                }

                if (type == typeof(BlockingState))
                {
                    return new BlockingState();
                }

                if (type == typeof(IEnumerable<ISubState<DebugCompositeState, EmptyPayload>>))
                {
                    return new ISubState<DebugCompositeState, EmptyPayload>[]
                    {
                        new DebugCompositeFirstSubState(),
                        new DebugCompositeSecondSubState()
                    };
                }

                throw new InvalidOperationException(type.FullName);
            }
        }

        private sealed class DebugFirstState : StateBase
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoTo<DebugSecondState>());
        }

        private sealed class DebugSecondState : StateBase
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoToExit());
        }

        private sealed class DebugFailingState : StateBase
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                throw new InvalidOperationException("Debug failure");
        }

        private sealed class DebugCompositeState : DefaultCompositeState
        {
        }

        private sealed class DebugCompositeFirstSubState : SubStateBase<DebugCompositeState>
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoToExit());
        }

        private sealed class DebugCompositeSecondSubState : SubStateBase<DebugCompositeState>
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoToExit());
        }

        private sealed class NestedParentState : StateBase
        {
            private readonly DebugResolver _resolver;

            public NestedParentState(DebugResolver resolver)
            {
                _resolver = resolver;
            }

            public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                var nestedMachine = new SilentStateMachine();
                nestedMachine.SetResolver(_resolver);
                nestedMachine.Execute<NestedChildState>(token).GetAwaiter().GetResult();

                return UniTask.FromResult(Transition.GoToExit());
            }
        }

        private sealed class NestedChildState : StateBase
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoToExit());
        }

        private sealed class BlockingState : StateBase
        {
            public static UniTaskCompletionSource<StateTransitionInfo> CompletionSource { get; set; }

            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                CompletionSource.Task;
        }
    }
}

#endif

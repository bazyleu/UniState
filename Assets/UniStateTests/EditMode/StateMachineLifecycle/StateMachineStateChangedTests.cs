using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;

namespace UniStateTests.EditMode.StateMachineLifecycle
{
    [TestFixture]
    public class StateMachineStateChangedTests
    {
        [Test]
        public void Execute_WithGoToAndExit_ReportsStateChanges()
        {
            var stateMachine = new TrackingStateMachine();
            stateMachine.SetResolver(new TestResolver());

            stateMachine.Execute<FirstState>(CancellationToken.None).GetAwaiter().GetResult();

            Assert.AreEqual(3, stateMachine.Changes.Count);

            Assert.AreEqual(StateMachineStateChangeType.Started, stateMachine.Changes[0].ChangeType);
            Assert.IsNull(stateMachine.Changes[0].PreviousStateType);
            Assert.AreEqual(typeof(FirstState), stateMachine.Changes[0].CurrentStateType);
            Assert.AreEqual(TransitionType.State, stateMachine.Changes[0].RequestedTransition.Transition);

            Assert.AreEqual(StateMachineStateChangeType.Changed, stateMachine.Changes[1].ChangeType);
            Assert.AreEqual(typeof(FirstState), stateMachine.Changes[1].PreviousStateType);
            Assert.AreEqual(typeof(SecondState), stateMachine.Changes[1].CurrentStateType);
            Assert.AreEqual(typeof(SecondState), stateMachine.Changes[1].CurrentTransition.Creator.StateType);
            Assert.AreEqual(TransitionType.State, stateMachine.Changes[1].RequestedTransition.Transition);

            Assert.AreEqual(StateMachineStateChangeType.Exited, stateMachine.Changes[2].ChangeType);
            Assert.AreEqual(typeof(SecondState), stateMachine.Changes[2].PreviousStateType);
            Assert.IsNull(stateMachine.Changes[2].CurrentStateType);
            Assert.IsNull(stateMachine.Changes[2].CurrentTransition);
            Assert.AreEqual(TransitionType.Exit, stateMachine.Changes[2].RequestedTransition.Transition);
        }

        [Test]
        public void Execute_WithGoBack_ReportsStateChangeFromHistory()
        {
            var stateMachine = new TrackingStateMachine();
            stateMachine.SetResolver(new TestResolver());

            stateMachine.Execute<BackFirstState>(CancellationToken.None).GetAwaiter().GetResult();

            Assert.AreEqual(4, stateMachine.Changes.Count);

            Assert.AreEqual(StateMachineStateChangeType.Started, stateMachine.Changes[0].ChangeType);
            Assert.AreEqual(typeof(BackFirstState), stateMachine.Changes[0].CurrentStateType);

            Assert.AreEqual(StateMachineStateChangeType.Changed, stateMachine.Changes[1].ChangeType);
            Assert.AreEqual(typeof(BackFirstState), stateMachine.Changes[1].PreviousStateType);
            Assert.AreEqual(typeof(BackSecondState), stateMachine.Changes[1].CurrentStateType);

            Assert.AreEqual(StateMachineStateChangeType.Changed, stateMachine.Changes[2].ChangeType);
            Assert.AreEqual(typeof(BackSecondState), stateMachine.Changes[2].PreviousStateType);
            Assert.AreEqual(typeof(BackFirstState), stateMachine.Changes[2].CurrentStateType);
            Assert.AreEqual(TransitionType.Back, stateMachine.Changes[2].RequestedTransition.Transition);
            Assert.AreEqual(typeof(BackFirstState), stateMachine.Changes[2].CurrentTransition.Creator.StateType);

            Assert.AreEqual(StateMachineStateChangeType.Exited, stateMachine.Changes[3].ChangeType);
            Assert.AreEqual(typeof(BackFirstState), stateMachine.Changes[3].PreviousStateType);
            Assert.IsNull(stateMachine.Changes[3].CurrentStateType);
        }

        [Test]
        public void Execute_WhenTokenCanceled_ReportsCanceledChange()
        {
            var resolver = new TestResolver();
            var stateMachine = new TrackingStateMachine();
            stateMachine.SetResolver(resolver);

            using var cts = new CancellationTokenSource();
            resolver.CancelScenario.Source = cts;

            var canceled = false;

            try
            {
                stateMachine.Execute<CancelingState>(cts.Token).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                canceled = true;
            }

            Assert.IsTrue(canceled);
            Assert.False(stateMachine.IsExecuting);

            Assert.AreEqual(2, stateMachine.Changes.Count);

            Assert.AreEqual(StateMachineStateChangeType.Started, stateMachine.Changes[0].ChangeType);
            Assert.AreEqual(typeof(CancelingState), stateMachine.Changes[0].CurrentStateType);

            Assert.AreEqual(StateMachineStateChangeType.Canceled, stateMachine.Changes[1].ChangeType);
            Assert.AreEqual(typeof(CancelingState), stateMachine.Changes[1].PreviousStateType);
            Assert.IsNull(stateMachine.Changes[1].CurrentStateType);
            Assert.IsNull(stateMachine.Changes[1].CurrentTransition);
            Assert.IsNull(stateMachine.Changes[1].RequestedTransition);
        }

        [Test]
        public void Execute_WhenStateChangedHandlerThrows_ClearsExecutionStatus()
        {
            var stateMachine = new ThrowingStateChangedStateMachine();
            stateMachine.SetResolver(new TestResolver());

            stateMachine.Execute<FirstState>(CancellationToken.None).GetAwaiter().GetResult();

            Assert.False(stateMachine.IsExecuting);
            Assert.NotNull(stateMachine.LastError);
            Assert.AreEqual(StateMachineErrorType.StateMachineFail, stateMachine.LastError.ErrorType);
        }

        [Test]
        public void Execute_WhenStateChangedHandlerThrowsOnChanged_DisposesCurrentStateOnce()
        {
            var resolver = new DisposeTrackingResolver();
            var stateMachine = new ThrowingOnChangedStateMachine();
            stateMachine.SetResolver(resolver);

            stateMachine.Execute<DisposeTrackingFirstState>(CancellationToken.None).GetAwaiter().GetResult();

            Assert.NotNull(stateMachine.LastError);
            Assert.AreEqual(StateMachineErrorType.StateMachineFail, stateMachine.LastError.ErrorType);
            Assert.AreEqual(1, resolver.FirstState.DisposeCount);
            Assert.AreEqual(1, resolver.SecondState.DisposeCount);
        }

        private sealed class TrackingStateMachine : StateMachine
        {
            public readonly List<StateMachineStateChangedData> Changes = new();

            protected override void HandleStateChanged(StateMachineStateChangedData changeData)
            {
                Changes.Add(changeData);
            }

            protected override void HandleError(StateMachineErrorData errorData)
            {
                throw new Exception("State machine error.", errorData.Exception);
            }
        }

        private sealed class ThrowingStateChangedStateMachine : StateMachine
        {
            public StateMachineErrorData LastError { get; private set; }

            protected override void HandleStateChanged(StateMachineStateChangedData changeData)
            {
                throw new InvalidOperationException("State changed handler failure.");
            }

            protected override void HandleError(StateMachineErrorData errorData)
            {
                LastError = errorData;
            }
        }

        private sealed class ThrowingOnChangedStateMachine : StateMachine
        {
            public StateMachineErrorData LastError { get; private set; }

            protected override void HandleStateChanged(StateMachineStateChangedData changeData)
            {
                if (changeData.ChangeType == StateMachineStateChangeType.Changed)
                {
                    throw new InvalidOperationException("State changed handler failure.");
                }
            }

            protected override void HandleError(StateMachineErrorData errorData)
            {
                LastError = errorData;
            }
        }

        private sealed class TestResolver : ITypeResolver
        {
            private readonly BackScenario _backScenario = new();

            public CancelScenario CancelScenario { get; } = new();

            public object Resolve(Type type)
            {
                if (type == typeof(FirstState))
                {
                    return new FirstState();
                }

                if (type == typeof(SecondState))
                {
                    return new SecondState();
                }

                if (type == typeof(BackFirstState))
                {
                    return new BackFirstState(_backScenario);
                }

                if (type == typeof(BackSecondState))
                {
                    return new BackSecondState();
                }

                if (type == typeof(CancelingState))
                {
                    return new CancelingState(CancelScenario);
                }

                throw new InvalidOperationException(type.FullName);
            }
        }

        private sealed class DisposeTrackingResolver : ITypeResolver
        {
            public DisposeTrackingFirstState FirstState { get; private set; }
            public DisposeTrackingSecondState SecondState { get; private set; }

            public object Resolve(Type type)
            {
                if (type == typeof(DisposeTrackingFirstState))
                {
                    return FirstState = new DisposeTrackingFirstState();
                }

                if (type == typeof(DisposeTrackingSecondState))
                {
                    return SecondState = new DisposeTrackingSecondState();
                }

                throw new InvalidOperationException(type.FullName);
            }
        }

        private sealed class FirstState : StateBase
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoTo<SecondState>());
        }

        private sealed class SecondState : StateBase
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoToExit());
        }

        private sealed class BackScenario
        {
            public int FirstStateExecutions { get; set; }
        }

        private sealed class CancelScenario
        {
            public CancellationTokenSource Source { get; set; }
        }

        private sealed class CancelingState : StateBase
        {
            private readonly CancelScenario _scenario;

            public CancelingState(CancelScenario scenario)
            {
                _scenario = scenario;
            }

            public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                _scenario.Source.Cancel();
                token.ThrowIfCancellationRequested();

                return UniTask.FromResult(Transition.GoToExit());
            }
        }

        private sealed class BackFirstState : StateBase
        {
            private readonly BackScenario _scenario;

            public BackFirstState(BackScenario scenario)
            {
                _scenario = scenario;
            }

            public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                _scenario.FirstStateExecutions++;

                return UniTask.FromResult(_scenario.FirstStateExecutions == 1
                    ? Transition.GoTo<BackSecondState>()
                    : Transition.GoToExit());
            }
        }

        private sealed class BackSecondState : StateBase
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoBack());
        }

        private sealed class DisposeTrackingFirstState : StateBase
        {
            public int DisposeCount { get; private set; }

            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoTo<DisposeTrackingSecondState>());

            public override void Dispose()
            {
                DisposeCount++;
                base.Dispose();
            }
        }

        private sealed class DisposeTrackingSecondState : StateBase
        {
            public int DisposeCount { get; private set; }

            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoToExit());

            public override void Dispose()
            {
                DisposeCount++;
                base.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using Zenject;

namespace UniStateTests.EditMode.Disposables
{
    [TestFixture]
    internal class StateDisposablesTests: ZenjectTestsBase
    {
        internal class TestStateMachine : StateMachine
        {
            protected override void HandleError(StateMachineErrorData errorData)
            {
                // Do nothing
            }
        }

        private class DisposablesState : StateBase<IList<IDisposable>>
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                Disposables.AddRange(Payload);

                return UniTask.FromResult(Transition.GoToExit());
            }
        }

        private class ExceptionDisposableState : DisposablesState
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                _ = base.Execute(token);

                throw new("Test exception");
            }
        }

        private class ManualDisposablesState : StateBase
        {
            public void AddDisposable(Action action) => Disposables.Add(action);

            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                UniTask.FromResult(Transition.GoToExit());
        }

        [Test]
        public void Dispose_DisposesInternalList() => CheckDisposeIsCalled<DisposablesState>();

        [Test]
        public void Execute_WithException_DisposesInternalList() => CheckDisposeIsCalled<ExceptionDisposableState>();

        [Test]
        public void Dispose_WhenDisposableThrows_DisposesRemainingAndSecondDisposeIsNoOp()
        {
            var disposeCount = 0;
            var state = new ManualDisposablesState();

            state.AddDisposable(() => disposeCount++);
            state.AddDisposable(() => throw new InvalidOperationException("Dispose failure"));

            Assert.Throws<InvalidOperationException>(() => state.Dispose());
            Assert.AreEqual(1, disposeCount);

            Assert.DoesNotThrow(() => state.Dispose());
            Assert.AreEqual(1, disposeCount);
        }

        private void CheckDisposeIsCalled<TState>()
            where TState : DisposablesState
        {
            var disposedObjects = 0;
            var disposables = new List<IDisposable>
            {
                new DisposableAction(() => disposedObjects++),
                () => disposedObjects++
            };

            ExecuteState<TState>(disposables);

            Assert.AreEqual(disposedObjects, 2);
        }

        private void ExecuteState<TState>(IList<IDisposable> disposables)
            where TState: DisposablesState
        {
            var stateMachine = Container.Resolve<IStateMachine>();
            stateMachine.Execute<TState, IList<IDisposable>>(disposables, default).GetAwaiter().GetResult();
        }

        protected override void SetupBindings(DiContainer container)
        {
            container.BindStateMachine<IStateMachine, TestStateMachine>();
            container.BindState<DisposablesState>();
            container.BindState<ExceptionDisposableState>();
        }
    }
}
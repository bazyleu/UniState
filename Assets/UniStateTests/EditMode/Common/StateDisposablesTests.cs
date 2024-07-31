using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using Zenject;

namespace UniStateTests.EditMode.Common
{
    [TestFixture]
    internal class StateDisposablesTests
    {
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

        [Test]
        public void Dispose_DisposesInternalList() => CheckDisposeIsCalled<DisposablesState>();

        [Test]
        public void Execute_WithException_DisposesInternalList() => CheckDisposeIsCalled<ExceptionDisposableState>();

        private static void CheckDisposeIsCalled<TState>()
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

        private static void ExecuteState<TState>(IList<IDisposable> disposables)
            where TState: DisposablesState
        {
            var container = new DiContainer(StaticContext.Container);

            container.Bind<StateMachine>().ToSelf().AsTransient();
            container.Bind<TState>().ToSelf().AsTransient();

            var stateMachine = StateMachineHelper.CreateStateMachine<StateMachine>(container.ToTypeResolver());

            stateMachine.Execute<TState, IList<IDisposable>>(disposables, default).GetAwaiter().GetResult();
        }
    }
}
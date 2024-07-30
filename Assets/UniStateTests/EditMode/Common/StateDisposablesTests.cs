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

        [Test]
        public void Dispose_DisposesInternalList()
        {
            var disposedObjects = 0;
            var disposables = new List<IDisposable>
            {
                new DisposableAction(() => disposedObjects++),
                () => disposedObjects++
            };

            ExecuteState(disposables);

            Assert.AreEqual(disposedObjects, 2);
        }

        private static void ExecuteState(IList<IDisposable> disposables)
        {
            var container = new DiContainer(StaticContext.Container);

            container.Bind<StateMachine>().ToSelf().AsTransient();
            container.Bind<DisposablesState>().ToSelf().AsTransient();
            
            var stateMachine =  StateMachineHelper.CreateStateMachine<StateMachine>(container.ToTypeResolver());

            stateMachine.Execute<DisposablesState, IList<IDisposable>>(disposables, default).GetAwaiter().GetResult();
        }
    }
}
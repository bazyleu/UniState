using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UniStateTests.Common
{
    public abstract class VContainerTestsBase : TestsBase
    {
        private GameObject _containerHolder;
        private IObjectResolver _objectResolver;

        protected IObjectResolver Container => _objectResolver;

        public override void Setup()
        {
            base.Setup();

            _containerHolder = new GameObject("container");
            var component = _containerHolder.AddComponent<TestsLifetimeScope>();
            var testScope = component.CreateChild(SetupBindings);
            _objectResolver = testScope.Container;
        }

        public override void TearDown()
        {
            base.TearDown();

            Object.Destroy(_containerHolder);
        }

        protected async UniTask RunAndVerify<TStateMachine, TState>()
            where TStateMachine : class, IStateMachine, IVerifiableStateMachine
            where TState : class, IState<EmptyPayload>
        {
            await StateMachineTestHelper.RunAndVerify<TStateMachine, TState>(Container.ToTypeResolver(),
                GetTimeoutToken());
        }

        private class TestsLifetimeScope : LifetimeScope
        {
        }

        protected virtual void SetupBindings(IContainerBuilder builder)
        {
            builder.Register<ExecutionLogger>(Lifetime.Singleton);
        }
    }
}
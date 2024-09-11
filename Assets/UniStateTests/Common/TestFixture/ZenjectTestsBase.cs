using Cysharp.Threading.Tasks;
using UniState;
using Zenject;

namespace UniStateTests.Common
{
    public abstract class ZenjectTestsBase : TestsBase
    {
        private DiContainer _container;

        protected DiContainer Container => _container;

        public override void Setup()
        {
            base.Setup();

            _container = new DiContainer(StaticContext.Container);

            SetupBindings(Container);
        }

        public override void TearDown()
        {
            base.TearDown();

            StaticContext.Clear();
        }

        protected async UniTask RunAndVerify<TStateMachine, TState>()
            where TStateMachine : class, IStateMachine, IVerifiableStateMachine
            where TState : class, IState<EmptyPayload>
        {
            await StateMachineTestHelper.RunAndVerify<TStateMachine, TState>(Container.ToTypeResolver(),
                GetTimeoutToken());
        }

        protected virtual void SetupBindings(DiContainer container)
        {
            container.BindInterfacesAndSelfTo<ExecutionLogger>().AsSingle();
        }
    }
}
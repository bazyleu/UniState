using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.GoBackToTests.Infrastructure;
using Zenject;

namespace UniStateTests.PlayMode.GoBackToTests
{
    [TestFixture]
    internal class GoBackToZenjectTests : ZenjectTestsBase
    {
        [Test]
        public void RunChainOfStates_GoBackToChain_LogsExpected()
            => RunAndVerify<IVerifiableStateMachine, GoBackToState1>().GetAwaiter().GetResult();

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.BindStateMachine<IVerifiableStateMachine, GoBackToStateMachine>();
            container.BindState<GoBackToState1>();
            container.BindState<GoBackToState2>();
            container.BindState<GoBackToState3>();
            container.BindState<GoBackToState4>();
            container.Bind<GoBackToTestsHelper>().AsSingle();
        }
    }
}
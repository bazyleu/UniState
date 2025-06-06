using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.GoBackToTests.Infrastructure;
using VContainer;

namespace UniStateTests.PlayMode.GoBackToTests
{
    internal class GoBackToVContainerTests : VContainerTestsBase
    {
        [Test]
        public void RunChainOfStates_GoBackToChain_LogsExpected()
            => RunAndVerify<IVerifiableStateMachine, GoBackToState1>().GetAwaiter().GetResult();

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterStateMachine<IVerifiableStateMachine, GoBackToStateMachine>();
            builder.RegisterState<GoBackToState1>();
            builder.RegisterState<GoBackToState2>();
            builder.RegisterState<GoBackToState3>();
            builder.RegisterState<GoBackToState4>();
            builder.Register<GoBackToTestsHelper>(Lifetime.Singleton);
        }
    }
}
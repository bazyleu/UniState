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
        private StateTransitionFacade _transitionFacade;

        private GoBackToTestsHelper Helper => Container.Resolve<GoBackToTestsHelper>();

        [Test]
        public void RunChainOfStates_GoBackToChain_SkipsToTargetState()
        {
            AddTransitionInfo("4", _transitionFacade.GoBack());
            AddTransitionInfo("3", _transitionFacade.GoBackTo<GoBackToTestState<string>>());
            AddTransitionInfo("2", _transitionFacade.GoTo<GoBackToTestState<float>>());
            AddTransitionInfo("1", _transitionFacade.GoTo<GoBackToTestState<int>>());
            Helper.ExpectedLog = "String (1) -> Int32 (2) -> Single (3) -> String (4)";

            RunAndVerify<GoBackToStateMachine, GoBackToTestState<string>>().GetAwaiter().GetResult();
        }

        [Test]
        public void RunChainOfStates_GoBackToChain_NoTargetState_ExitsStateMachine()
        {
            AddTransitionInfo("4", _transitionFacade.GoBack());
            AddTransitionInfo("3", _transitionFacade.GoBackTo<GoBackToTestState<double>>());
            AddTransitionInfo("2", _transitionFacade.GoTo<GoBackToTestState<float>>());
            AddTransitionInfo("1", _transitionFacade.GoTo<GoBackToTestState<int>>());
            Helper.ExpectedLog = "String (1) -> Int32 (2) -> Single (3)";

            RunAndVerify<GoBackToStateMachine, GoBackToTestState<string>>().GetAwaiter().GetResult();
        }

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.BindStateMachine<GoBackToStateMachine>();
            container.Bind(typeof(GoBackToTestState<>)).AsTransient();
            container.Bind<GoBackToTestsHelper>().AsSingle();

            _transitionFacade = new(new StateTransitionFactory(container.ToTypeResolver()));
        }

        private void AddTransitionInfo(string log, StateTransitionInfo info)
            => Helper.TransitionsStack.Push(new(log, info));
    }
}
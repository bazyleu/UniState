using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackToTests.Infrastructure
{
    internal class GoBackToStateMachine : VerifiableStateMachine
    {
        private readonly GoBackToTestsHelper _helper;

        public GoBackToStateMachine(GoBackToTestsHelper helper, ExecutionLogger logger)
            : base(logger)
        {
            _helper = helper;
        }

        protected override string ExpectedLog => _helper.ExpectedLog;
    }
}
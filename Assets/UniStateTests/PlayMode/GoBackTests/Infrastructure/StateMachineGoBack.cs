using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackTests.Infrastructure
{
    internal class StateMachineGoBack : VerifiableStateMachine
    {
        protected override string ExpectedLog =>
            "StateGoBack1 (Execute) -> StateGoBack2 (Execute:42) -> StateGoBack3 (Execute) -> " +
            "StateGoBack2 (Execute:42) -> StateGoBack1 (Execute)";

        public StateMachineGoBack(ExecutionLogger logger) : base(logger)
        {
        }
    }
}
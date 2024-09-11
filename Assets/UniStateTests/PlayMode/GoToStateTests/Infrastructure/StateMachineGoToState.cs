using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal class StateMachineGoToState : VerifiableStateMachine
    {
        public StateMachineGoToState(ExecutionLogger logger) : base(logger)
        {
        }

        protected override string ExpectedLog =>
            "StateGoTo1 (Execute) -> StateGoTo2 (Execute) -> StateGoTo3 (Execute) -> " +
            "StateGoTo4 (Execute) -> StateGoTo5 (Execute:42) -> SubStateGoToX6A (Execute) -> SubStateGoToX6B (Execute)";
    }
}
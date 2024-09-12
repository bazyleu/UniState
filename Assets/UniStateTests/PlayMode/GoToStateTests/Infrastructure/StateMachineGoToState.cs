using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal class StateMachineGoToState : VerifiableStateMachine
    {
        protected override string ExpectedLog =>
            "StateGoTo1 (Execute) -> StateGoTo2 (Execute) -> StateGoTo3 (Execute) -> " +
            "StateGoTo4 (Execute) -> StateGoTo5 (Execute:42) -> SubStateGoToX6A (Execute) -> SubStateGoToX6B (Execute) -> " +
            "SubStateGoToX7A (Execute:True) -> SubStateGoToX7B (Execute:True) -> StateGoTo8 (Execute:False) -> " +
            "SubStateGoToX7A (Execute:False) -> SubStateGoToX7B (Execute:False) -> StateGoTo8 (Execute:True)";

        public StateMachineGoToState(ExecutionLogger logger) : base(logger)
        {
        }
    }
}
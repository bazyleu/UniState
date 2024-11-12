using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal class StateMachineGoToState : VerifiableStateMachine
    {
        protected override string ExpectedLog =>
            "StateGoTo1 (Execute) -> StateGoTo2 (Execute) -> StateGoTo3 (Execute) -> " +
            "StateGoTo4 (Execute) -> StateGoTo5 (Execute:42) -> SubStateGoTo6First (Execute) -> SubStateGoTo6Second (Execute) -> " +
            "SubStateGoTo7First (Execute:True) -> SubStateGoTo7Second (Execute:True) -> StateGoTo8 (Execute:False) -> " +
            "SubStateGoTo7First (Execute:False) -> SubStateGoTo7Second (Execute:False) -> StateGoTo8 (Execute:True)";

        public StateMachineGoToState(ExecutionLogger logger) : base(logger)
        {
        }
    }
}
using UniStateTests.Common;

namespace UniStateTests.PlayMode.HistoryTests.Infrastructure
{
    internal class StateMachineZeroHistory : VerifiableStateMachine
    {
        public const int MaxTransition = 6;

        protected override int MaxHistorySize => 0;

        protected override string ExpectedLog =>
            "StateInitZeroHistory (Execute) -> " +
            "StateFooHistory (False) -> StateBarHistory (False) -> StateFooHistory (False) -> StateBarHistory (False) -> " +
            "StateFooHistory (False) -> StateBarHistory (True)";

        public StateMachineZeroHistory(ExecutionLogger logger) : base(logger)
        {
        }
    }
}
using UniStateTests.Common;

namespace UniStateTests.PlayMode.HistoryTests.Infrastructure
{
    internal class StateMachineLongHistory : VerifiableStateMachine
    {
        public const int MaxTransition = 24;

        protected override int MaxHistorySize => 10;

        protected override string ExpectedLog =>
            // GoTO
            "StateInitLongHistory (Execute) -> " +
            "StateFooHistory (False) -> StateBarHistory (False) -> StateFooHistory (False) -> StateBarHistory (False) -> " +
            "StateFooHistory (False) -> StateBarHistory (False) -> StateFooHistory (False) -> StateBarHistory (False) -> " +
            "StateFooHistory (False) -> StateBarHistory (False) -> StateFooHistory (False) -> StateBarHistory (False) -> " +
            "StateFooHistory (False) -> StateBarHistory (False) -> StateFooHistory (False) -> StateBarHistory (False) -> " +
            "StateFooHistory (False) -> StateBarHistory (False) -> StateFooHistory (False) -> StateBarHistory (False) -> " +
            "StateFooHistory (False) -> StateBarHistory (False) -> StateFooHistory (False) -> StateBarHistory (True) -> " +
            //GOBack
            "StateFooHistory (True) -> StateBarHistory (True) -> StateFooHistory (True) -> StateBarHistory (True) -> " +
            "StateFooHistory (True) -> StateBarHistory (True) -> StateFooHistory (True) -> StateBarHistory (True) -> " +
            "StateFooHistory (True) -> StateBarHistory (True)";

        public StateMachineLongHistory(ExecutionLogger logger) : base(logger)
        {
        }
    }
}
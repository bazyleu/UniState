using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackTests.Infrastructure
{
    internal class StateMachineGoBack : VerifiableStateMachine
    {
        protected override string ExpectedLog =>
            "StateGoBackFirst (Execute) -> StateGoBackSecond (Execute:42) -> StateGoBackThird (Execute) -> " +
            "StateGoBackSecond (Execute:42) -> StateGoBackFirst (Execute)";

        public StateMachineGoBack(ExecutionLogger logger) : base(logger)
        {
        }
    }
}
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackToTests.Infrastructure
{
    internal class GoBackToStateMachine : VerifiableStateMachine
    {
        public GoBackToStateMachine(ExecutionLogger logger)
            : base(logger)
        { }

        protected override string ExpectedLog =>
            $"{nameof(GoBackToState1)} (Execute) -> {nameof(GoBackToState2)} (Execute:42, Execute:11) -> " +
            $"{nameof(GoBackToState3)} (Execute) -> {nameof(GoBackToState4)} (Execute) -> " +
            $"{nameof(GoBackToState2)} (Execute:11)";
    }
}
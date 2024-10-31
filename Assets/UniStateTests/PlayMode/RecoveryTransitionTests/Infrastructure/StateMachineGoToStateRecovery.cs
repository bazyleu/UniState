using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure
{
    internal class StateMachineGoToStateRecovery : VerifiableStateMachine
    {
        private readonly ExecutionLogger _logger;

        public StateMachineGoToStateRecovery(ExecutionLogger logger) : base(logger)
        {
            _logger = logger;
        }

        protected override void HandleError(StateMachineErrorData errorData)
        {
            _logger.LogStep("StateMachineGoToStateRecovery", $"HandleError ({errorData.Exception.Message})");
        }

        protected override StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory)
            => transitionFactory.CreateStateTransition<StateMagicRecovery>();

        protected override string ExpectedLog =>
            "StateInitRecovery (Execute) -> " +
            "StateThrowTwoExceptionRecovery (Initialize) -> StateMachineGoToStateRecovery (HandleError (Initialize exception)) -> " +
            "StateThrowTwoExceptionRecovery (Execute, Exit) -> StateMachineGoToStateRecovery (HandleError (Exit exception)) -> " +
            "StateFailExecutionRecovery (Execute) -> StateMachineGoToStateRecovery (HandleError (Execution exception)) -> " +
            "StateFailExecutionRecovery (Exit, Disposables) -> StateMagicRecovery (Execute)";
    }
}
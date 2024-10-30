using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure
{
    internal class StateMachineDefaultRecovery : VerifiableStateMachine
    {
        private readonly ExecutionLogger _logger;

        public StateMachineDefaultRecovery(ExecutionLogger logger) : base(logger)
        {
            _logger = logger;
        }

        protected override void HandleError(StateMachineErrorData errorData)
        {
            _logger.LogStep("StateMachineDefaultRecovery", $"HandleError ({errorData.Exception.Message})");
        }

        protected override string ExpectedLog =>
            "StateInitRecovery (Execute) -> " +
            "StateThrowTwoExceptionRecovery (Initialize) -> StateMachineDefaultRecovery (HandleError (Initialize exception)) -> " +
            "StateThrowTwoExceptionRecovery (Execute, Exit) -> StateMachineDefaultRecovery (HandleError (Exit exception)) -> " +
            "StateFailExecutionRecovery (Execute) -> StateMachineDefaultRecovery (HandleError (Execution exception)) -> " +
            "StateThrowTwoExceptionRecovery (Initialize) -> StateMachineDefaultRecovery (HandleError (Initialize exception)) -> " +
            "StateThrowTwoExceptionRecovery (Execute, Exit) -> StateMachineDefaultRecovery (HandleError (Exit exception)) -> " +
            "StateFailExecutionRecovery (Execute)";
    }
}
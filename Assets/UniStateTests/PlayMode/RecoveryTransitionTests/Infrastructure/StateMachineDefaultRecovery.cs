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
            "StateInitial (Execute) -> " +
            "StateThrowTwoException (Initialize) -> StateMachineDefaultRecovery (HandleError (Initialize exception)) -> " +
            "StateThrowTwoException (Execute, Exit) -> StateMachineDefaultRecovery (HandleError (Exit exception)) -> " +
            "StateWithFailExecution (Execute) -> StateMachineDefaultRecovery (HandleError (Execution exception)) -> " +
            "StateWithFailExecution (Exit, Disposables) -> " +
            "StateThrowTwoException (Initialize) -> StateMachineDefaultRecovery (HandleError (Initialize exception)) -> " +
            "StateThrowTwoException (Execute, Exit) -> StateMachineDefaultRecovery (HandleError (Exit exception)) -> " +
            "StateWithFailExecution (Execute, Exit, Disposables)";
    }
}
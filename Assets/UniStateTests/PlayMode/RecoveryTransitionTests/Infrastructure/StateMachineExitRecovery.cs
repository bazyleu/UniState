using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure
{
    internal class StateMachineExitRecovery : VerifiableStateMachine
    {
        private readonly ExecutionLogger _logger;

        public StateMachineExitRecovery(ExecutionLogger logger) : base(logger)
        {
            _logger = logger;
        }

        protected override void HandleError(StateMachineErrorData errorData)
        {
            _logger.LogStep("StateMachineExitRecovery", $"HandleError ({errorData.Exception.Message})");
        }

        protected override StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory)
            => transitionFactory.CreateExitTransition();

        protected override string ExpectedLog =>
            "StateInitial (Execute) -> " +
            "StateThrowTwoException (Initialize) -> StateMachineExitRecovery (HandleError (Initialize exception)) -> " +
            "StateThrowTwoException (Execute, Exit) -> StateMachineExitRecovery (HandleError (Exit exception)) -> " +
            "StateWithFailExecution (Execute) -> StateMachineExitRecovery (HandleError (Execution exception)) -> " +
            "StateWithFailExecution (Exit, Disposables)";
    }
}
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure
{
    internal class StateMachineGoToStateRecovery : VerifiableStateMachine, IStateMachineGoToStateRecovery
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
            => transitionFactory.CreateStateTransition<StateStartedAfterException>();

        protected override string ExpectedLog =>
            "StateInitial (Execute) -> " +
            "StateThrowTwoException (Initialize) -> StateMachineGoToStateRecovery (HandleError (Initialize exception)) -> " +
            "StateThrowTwoException (Execute, Exit) -> StateMachineGoToStateRecovery (HandleError (Exit exception)) -> " +
            "StateWithFailExecution (Execute) -> StateMachineGoToStateRecovery (HandleError (Execution exception)) -> " +
            "StateWithFailExecution (Exit, Disposables) -> StateStartedAfterException (Execute)";
    }
    public interface IStateMachineGoToStateRecovery : IStateMachine
    {}

}
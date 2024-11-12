using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.SubStateTests.Infrastructure
{
    public class StateMachineSubStates : VerifiableStateMachine
    {
        public StateMachineSubStates(ExecutionLogger logger) : base(logger)
        {
        }

        protected override StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory)
            => transitionFactory.CreateExitTransition();

        protected override void HandleError(StateMachineErrorData errorData)
        {
        }

        protected override string ExpectedLog =>
            "SubStateInitialSecond (Execute) -> SubStateInitialFirst (Execute, Disposables) -> SubStateInitialSecond (Disposables) -> " +
            "SubStateFinalSecond (Execute) -> SubStateFinalFirst (Execute, Disposables) -> SubStateFinalSecond (Disposables)";
    }
}
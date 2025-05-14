using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.Execution.Infrastructure
{
    public class ExecutionStateMachine : VerifiableStateMachine
    {
        private readonly ExecutionTestHelper _executionTestHelper;

        public ExecutionStateMachine(ExecutionLogger logger, ExecutionTestHelper executionTestHelper) : base(logger)
        {
            _executionTestHelper = executionTestHelper;

            _executionTestHelper.SetCurrentStateMachine(this);
        }

        protected override void HandleError(StateMachineErrorData errorData)
        {
            //Do nothing
        }

        protected override StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory) =>
            transitionFactory.CreateExitTransition();

        protected override string ExpectedLog
        {
            get
            {
                return _executionTestHelper.ExecutionType switch
                {
                    StateMachineExecutionType.Default => "FirstState (True) -> SecondState (True)",
                    StateMachineExecutionType.WrongDependency => "FirstState (True)",
                    StateMachineExecutionType.Exception => "FirstState (True) -> SecondStateWithException (True)",
                    _ => "-"
                };
            }
        }
    }
}
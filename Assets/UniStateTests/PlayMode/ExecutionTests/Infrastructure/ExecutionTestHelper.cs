using UniStateTests.Common;

namespace UniStateTests.PlayMode.Execution.Infrastructure
{
    public class ExecutionTestHelper
    {
        public IVerifiableStateMachine CurrentStateMachine { get; private set; }
        public StateMachineExecutionType ExecutionType { get; private set; }

        public void SetCurrentStateMachine(IVerifiableStateMachine stateMachine)
        {
            CurrentStateMachine = stateMachine;
        }

        public void SetPath(StateMachineExecutionType executionType)
        {
            ExecutionType = executionType;
        }
    }
}
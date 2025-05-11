using UniStateTests.Common;

namespace UniStateTests.PlayMode.StateMachineTests.Infrastructure
{
    public class StateMachineTestHelper
    {
        private IVerifiableStateMachine _currentStateMachine;

        public void SetCurrentStateMachine(IVerifiableStateMachine stateMachine)
        {
            _currentStateMachine = stateMachine;
        }
    }
}
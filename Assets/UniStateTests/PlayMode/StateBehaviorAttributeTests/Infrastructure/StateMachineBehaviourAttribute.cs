using UniStateTests.Common;

namespace UniStateTests.PlayMode.StateBehaviorAttributeTests.Infrastructure
{
    public class StateMachineBehaviourAttribute : VerifiableStateMachine
    {
        protected override string ExpectedLog =>
            "FirstState (Initialize, Execute, Exit) -> NoReturnState (Initialize, Execute) -> " +
            "FastInitializeState (Initialize) -> NoReturnState (Exit) -> FastInitializeState (Execute, Exit) -> " +
            "FirstState (Initialize, Execute, Exit)";

        public StateMachineBehaviourAttribute(ExecutionLogger logger) : base(logger)
        {
        }
    }
}
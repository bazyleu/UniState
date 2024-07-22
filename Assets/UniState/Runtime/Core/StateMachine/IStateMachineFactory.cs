namespace UniState
{
    public interface IStateMachineFactory
    {
        IExecutableStateMachine Create<TSateMachine>(ITypeResolver typeResolver)
            where TSateMachine : class, IStateMachine;

        IExecutableStateMachine Create<TSateMachine>()
            where TSateMachine : class, IStateMachine;
    }
}
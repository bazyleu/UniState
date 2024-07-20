namespace UniState
{
    public interface IStateMachineFactory
    {
        IExecutableStateMachine Create<TSateMachine>(ITypeResolver typeResolver)
            where TSateMachine : class, IStateMachine;

        IExecutableStateMachine Create<TSateMachine>()
            where TSateMachine : class, IStateMachine;

#if UNISTATE_VCONTAINER_SUPPORT
        IExecutableStateMachine Create<TSateMachine>(VContainer.IObjectResolver objectResolver)
            where TSateMachine : class, IStateMachine;
#endif
    }
}
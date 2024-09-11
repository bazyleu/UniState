namespace UniState
{
    public interface IStateMachineFactory
    {
        IExecutableStateMachine Create<TSateMachine>(ITypeResolver typeResolver)
            where TSateMachine : class, IStateMachine;

        IExecutableStateMachine Create<TSateMachine>()
            where TSateMachine : class, IStateMachine;

        public TReturn Create<TSateMachine, TReturn>(ITypeResolver typeResolver)
            where TSateMachine : class, IStateMachine, TReturn
            where TReturn : IExecutableStateMachine;

        public TReturn Create<TSateMachine, TReturn>()
            where TSateMachine : class, IStateMachine, TReturn
            where TReturn : IExecutableStateMachine;
    }
}
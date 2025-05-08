namespace UniState
{
    public static class StateMachineHelper
    {
        public static IExecutableStateMachine CreateStateMachine<TSateMachine>(ITypeResolver typeResolver)
            where TSateMachine : class, IStateMachine
        {
            var stateMachine = typeResolver.Resolve<TSateMachine>();

            stateMachine.SetResolver(typeResolver);

            return stateMachine;
        }

        public static TReturn CreateStateMachine<TSateMachine, TReturn>(ITypeResolver typeResolver)
            where TSateMachine : class, IStateMachine, TReturn
            where TReturn : IExecutableStateMachine
        {
            var stateMachine = typeResolver.Resolve<TSateMachine>();

            stateMachine.SetResolver(typeResolver);

            return stateMachine;
        }

    }
}
namespace UniState
{
    public static class StateMachineHelper
    {
        public static TStateMachine CreateStateMachine<TStateMachine>(ITypeResolver typeResolver)
            where TStateMachine : IStateMachine
        {
            var stateMachine = typeResolver.Resolve<TStateMachine>();

            stateMachine.SetResolver(typeResolver);

            return stateMachine;
        }

        public static TInterface CreateStateMachine<TInterface, TStateMachine>(ITypeResolver typeResolver)
            where TStateMachine : TInterface
            where TInterface : IStateMachine
        {
            var stateMachine = typeResolver.Resolve<TStateMachine>();

            stateMachine.SetResolver(typeResolver);

            return stateMachine;
        }
    }
}
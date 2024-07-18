using UniState;

namespace UniState
{
    public static class StateMachineHelper
    {
        public static IExecutableStateMachine CreateStateMachine<TSateMachine>(ITypeResolver typeResolver)
            where TSateMachine : class, IStateMachine
        {
            var stateMachine = typeResolver.Resolve<TSateMachine>();

            stateMachine.Initialize(typeResolver);

            return stateMachine;
        }

    }
}
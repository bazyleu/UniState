#if UNISTATE_VCONTAINER_SUPPORT

using VContainer;

namespace UniState
{
    public static class VContainerBuilderExtensions
    {
        public static RegistrationBuilder RegisterStateMachine<TStateMachine>(this IContainerBuilder builder)
            where TStateMachine : IStateMachine =>
            builder.Register<TStateMachine>(Lifetime.Transient);

        public static RegistrationBuilder RegisterAbstractStateMachine<TStateMachineBase, TStateMachine>(this IContainerBuilder builder)
            where TStateMachine : TStateMachineBase
            where TStateMachineBase : IStateMachine =>
            builder.Register<TStateMachineBase, TStateMachine>(Lifetime.Transient);

        public static RegistrationBuilder RegisterState<TState>(this IContainerBuilder builder) =>
            builder.Register<TState>(Lifetime.Transient).AsSelf().AsImplementedInterfaces();

        public static RegistrationBuilder RegisterAbstractState<TStateBase, TState>(this IContainerBuilder builder)
            where TState : TStateBase =>
            builder.Register<TStateBase, TState>(Lifetime.Transient);
    }
}

#endif
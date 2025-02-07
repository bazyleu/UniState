#if UNISTATE_VCONTAINER_SUPPORT

using VContainer;

namespace UniState
{
    public static class VContainerBuilderExtensions
    {
        public static RegistrationBuilder RegisterStateMachine<TStateMachine>(this IContainerBuilder builder)
            where TStateMachine : IStateMachine =>
            builder.RegisterStateMachine<TStateMachine>(Lifetime.Transient);

        public static RegistrationBuilder RegisterStateMachine<TStateMachine>(this IContainerBuilder builder,
            Lifetime lifetime)
            where TStateMachine : IStateMachine =>
            builder.Register<TStateMachine>(lifetime);

        public static RegistrationBuilder RegisterAbstractStateMachine<TStateMachineBase, TStateMachine>(
            this IContainerBuilder builder)
            where TStateMachine : TStateMachineBase
            where TStateMachineBase : IStateMachine =>
            builder.RegisterAbstractStateMachine<TStateMachineBase, TStateMachine>(Lifetime.Transient);

        public static RegistrationBuilder RegisterAbstractStateMachine<TStateMachineBase, TStateMachine>(
            this IContainerBuilder builder, Lifetime lifetime)
            where TStateMachine : TStateMachineBase
            where TStateMachineBase : IStateMachine =>
            builder.Register<TStateMachineBase, TStateMachine>(lifetime);

        public static RegistrationBuilder RegisterState<TState>(this IContainerBuilder builder) =>
            builder.RegisterState<TState>(Lifetime.Transient);

        public static RegistrationBuilder RegisterState<TState>(this IContainerBuilder builder, Lifetime lifetime) =>
            builder.Register<TState>(lifetime).AsSelf().AsImplementedInterfaces();

        public static RegistrationBuilder RegisterAbstractState<TStateBase, TState>(this IContainerBuilder builder)
            where TState : TStateBase =>
            builder.RegisterAbstractState<TStateBase, TState>(Lifetime.Transient);

        public static RegistrationBuilder RegisterAbstractState<TStateBase, TState>(this IContainerBuilder builder,
            Lifetime lifetime)
            where TState : TStateBase =>
            builder.Register<TStateBase, TState>(lifetime);
    }
}

#endif
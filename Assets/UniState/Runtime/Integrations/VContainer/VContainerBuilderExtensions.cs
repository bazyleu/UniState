#if UNISTATE_VCONTAINER_SUPPORT

using VContainer;

namespace UniState
{       //TODO: Change API for regisreing State
    public static class VContainerBuilderExtensions
    {
        public static void RegisterStateMachine<TInterface, TStateMachine>(
            this IContainerBuilder builder,
            Lifetime lifetime)
            where TStateMachine : TInterface
            where TInterface : IStateMachine
        {
            builder.Register<TStateMachine>(lifetime);
            builder.Register<TInterface>(
                r =>
                {
                    var stateMachine = r.Resolve<TStateMachine>();
                    stateMachine.SetResolver(r.ToTypeResolver());

                    return stateMachine;
                },
                lifetime);
        }

        public static void RegisterStateMachine<TInterface, TStateMachine>(
            this IContainerBuilder builder)
            where TStateMachine : TInterface
            where TInterface : IStateMachine
        {
            builder.RegisterStateMachine<TInterface, TStateMachine>(Lifetime.Transient);
        }

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
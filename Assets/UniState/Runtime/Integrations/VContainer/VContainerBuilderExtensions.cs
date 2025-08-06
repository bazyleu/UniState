#if UNISTATE_VCONTAINER_SUPPORT

using System;
using VContainer;

namespace UniState
{
    public static class VContainerBuilderExtensions
    {
        public static void RegisterStateMachine<TInterface, TStateMachine>(
            this IContainerBuilder builder,
            Lifetime lifetime)
            where TStateMachine : TInterface
            where TInterface : IStateMachine
        {
            if (typeof(TInterface) == typeof(TStateMachine))
                throw new ArgumentException(
                    $"RegisterStateMachine<{typeof(TInterface).Name}>: Type parameters must differ : use RegisterStateMachine<Interface, Implementation>() where Implementation implements Interface.\");");

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

        public static void RegisterState<TState>(this IContainerBuilder builder)
            where TState : IExecutableState =>
            builder.RegisterState<TState>(Lifetime.Transient);

        public static void RegisterState<TState>(this IContainerBuilder builder, Lifetime lifetime)
            where TState : IExecutableState =>
            builder.Register<TState>(lifetime).AsSelf().AsImplementedInterfaces();

        public static void RegisterState<TInterface, TState>(this IContainerBuilder builder)
            where TState : TInterface, IExecutableState =>
            builder.RegisterState<TInterface, TState>(Lifetime.Transient);

        public static void RegisterState<TInterface, TState>(this IContainerBuilder builder,
            Lifetime lifetime)
            where TState : TInterface, IExecutableState =>
            builder.Register<TInterface, TState>(lifetime);
    }
}

#endif
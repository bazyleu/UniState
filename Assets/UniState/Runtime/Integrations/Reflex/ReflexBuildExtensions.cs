using System;
using Reflex.Core;

namespace UniState
{
    public static class ReflexBuildExtensions
    {
        public static void AddStateMachine<TInterface, TStateMachine>(
            this ContainerBuilder builder)
            where TStateMachine : TInterface, new()
            where TInterface : IStateMachine
        {
            if (typeof(TInterface) == typeof(TStateMachine))
                throw new ArgumentException(
                    $"RegisterStateMachine<{typeof(TInterface).Name}>: Type parameters must differ : use AddStateMachine<Interface, Implementation>() where Implementation implements Interface.\");");

            builder.AddTransient(container =>
            {
                TStateMachine stateMachine = new();
                stateMachine.SetResolver(container.ToTypeResolver());

                return stateMachine;
            }, typeof(TInterface), typeof(TStateMachine));
        }

        public static void AddSingletonStateMachine<TInterface, TStateMachine>(
            this ContainerBuilder builder)
            where TStateMachine : TInterface, new()
            where TInterface : IStateMachine
        {
            if (typeof(TInterface) == typeof(TStateMachine))
                throw new ArgumentException(
                    $"RegisterStateMachine<{typeof(TInterface).Name}>: Type parameters must differ : use AddSingletonStateMachine<Interface, Implementation>() where Implementation implements Interface.\");");

            builder.AddSingleton(container =>
            {
                TStateMachine stateMachine = new();
                stateMachine.SetResolver(container.ToTypeResolver());

                return stateMachine;
            }, typeof(TInterface),  typeof(TStateMachine));
        }

        public static void AddState<TInterface, TState>(this ContainerBuilder builder)
            where TState : TInterface =>
            builder.AddTransient(typeof(TState), typeof(TInterface), typeof(TState));

        public static void AddSingletonState<TInterface, TState>(this ContainerBuilder builder)
            where TState : TInterface =>
            builder.AddSingleton(typeof(TState), typeof(TInterface),  typeof(TState));
    }
}

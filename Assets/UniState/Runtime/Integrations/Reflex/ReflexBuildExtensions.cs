#if UNISTATE_REFLEX_SUPPORT

using System;
using Reflex.Core;

namespace UniState
{
    public static class ReflexBuildExtensions
    {
        // builder.AddStateMachine<TInterface, TStateMachine>();
        public static void AddStateMachine<TInterface, TStateMachine>(
            this ContainerBuilder builder)
            where TStateMachine : TInterface, new()
            where TInterface : IStateMachine
        {
            if (typeof(TInterface) == typeof(TStateMachine))
                throw new ArgumentException(
                    $"RegisterStateMachine<{typeof(TInterface).Name}>: Type parameters must differ : " +
                    "use AddStateMachine<Interface, Implementation>() where Implementation implements Interface.\");");

            builder.AddTransient(container =>
            {
                TStateMachine stateMachine = new();
                stateMachine.SetResolver(container.ToTypeResolver());
                
                return stateMachine;
            }, typeof(TInterface), typeof(TStateMachine));
        }

        
        
        // builder.AddSingletonStateMachine<TInterface, TStateMachine>();
        public static void AddSingletonStateMachine<TInterface, TStateMachine>(
            this ContainerBuilder builder)
            where TStateMachine : TInterface, new()
            where TInterface : IStateMachine
        {
            if (typeof(TInterface) == typeof(TStateMachine))
                throw new ArgumentException(
                    $"RegisterStateMachine<{typeof(TInterface).Name}>: Type parameters must differ : " +
                    "use AddSingletonStateMachine<Interface, Implementation>() where Implementation implements Interface.\");");

            builder.AddSingleton(container =>
            {
                TStateMachine stateMachine = new();
                stateMachine.SetResolver(container.ToTypeResolver());

                return stateMachine;
            }, typeof(TInterface),  typeof(TStateMachine));
        }
        
        
        
        // builder.AddState<TInterface, TState>();
        // Compile type check
        public static void AddState<TInterface, TState>(this ContainerBuilder builder)
            where TState : TInterface =>
            builder.AddTransient(typeof(TState), typeof(TInterface), typeof(TState));
        
        // builder.AddState<TInterface>(typeof(stateType));
        // Runtime type check
        public static void AddState<TInterface>(this ContainerBuilder builder, Type stateType)
        {
            if (!typeof(IExecutableState).IsAssignableFrom(typeof(TInterface)))
            {
                throw new ArgumentException(
                    $"AddState<{typeof(TInterface).Name}>({stateType.Name}): Type parameter stateType must implement a valid state : " +
                    "did you forget to extend it from IState<TPayload>?");
            }
            
            if (!typeof(TInterface).IsAssignableFrom(stateType))
            {
                throw new ArgumentException(
                    $"AddState<{typeof(TInterface).Name}>({stateType.Name}): TInterface must be assignable from stateType : " +
                    $"are you sure {stateType.Name} implements {typeof(TInterface).Name}?");
            }
            
            builder.AddTransient(stateType, typeof(TInterface), stateType);
        }
        
        // builder.AddState(typeof(stateType));
        // Only binds it's concrete type
        public static void AddState(this ContainerBuilder builder, Type stateType) {
            if (!typeof(IExecutableState).IsAssignableFrom(stateType))
            {
                throw new ArgumentException(
                    $"AddState({stateType.Name}): Type parameter stateType must implement a valid state : " +
                    "did you forget to extend it from IState<TPayload>?");
            }
            
            builder.AddTransient(stateType);
        }

        
        
        // builder.AddSingletonState<TInterface, TState>();
        // Compile type check
        public static void AddSingletonState<TInterface, TState>(this ContainerBuilder builder)
            where TState : TInterface =>
            builder.AddSingleton(typeof(TState), typeof(TInterface), typeof(TState));
        
        // builder.AddSingletonState<TInterface>(typeof(stateType));
        // Runtime type check
        public static void AddSingletonState<TInterface>(this ContainerBuilder builder, Type stateType)
        {
            if (!typeof(IExecutableState).IsAssignableFrom(typeof(TInterface)))
            {
                throw new ArgumentException(
                    $"AddSingletonState<{typeof(TInterface).Name}>({stateType.Name}): Type parameter stateType must implement a valid state : " +
                    "did you forget to extend it from IState<TPayload>?");
            }
            
            if (!typeof(TInterface).IsAssignableFrom(stateType))
            {
                throw new ArgumentException(
                    $"AddSingletonState<{typeof(TInterface).Name}>({stateType.Name}): TInterface must be assignable from stateType : " +
                    $"are you sure {stateType.Name} implements {typeof(TInterface).Name}?");
            }
            
            builder.AddSingleton(stateType, typeof(TInterface), stateType);
        }
        
        // builder.AddSingletonState(typeof(stateType));
        // Only binds it's concrete type
        public static void AddSingletonState(this ContainerBuilder builder, Type stateType) {
            if (!typeof(IExecutableState).IsAssignableFrom(stateType))
            {
                throw new ArgumentException(
                    $"AddSingletonState({stateType.Name}): Type parameter stateType must implement a valid state : " +
                    "did you forget to extend it from IState<TPayload>?");
            }
            
            builder.AddSingleton(stateType);
        }
    }
}

#endif
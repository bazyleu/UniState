#if UNISTATE_REFLEX_SUPPORT

using System;
using Reflex.Core;
using Reflex.Enums;

namespace UniState
{
    public static class ReflexBuildExtensions
    {
        public static void RegisterStateMachine(
            this ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract)
        {
            RegisterStateMachine(builder, stateMachineImplementation, stateMachineContract, Lifetime.Transient);
        }

        public static void RegisterStateMachine(
            this ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract,
            Lifetime lifetime)
        {
            RegisterStateMachineInternal(builder, stateMachineImplementation, stateMachineContract, lifetime);
        }

        public static void RegisterState(this ContainerBuilder builder, Type state)
        {
            RegisterState(builder, state, Lifetime.Transient);
        }

        public static void RegisterState(this ContainerBuilder builder, Type state, Lifetime lifetime)
        {
            ValidateStateBindingInput(state);

            builder.RegisterType(state, GetStateContracts(state), lifetime, Resolution.Lazy);
        }

        public static void RegisterState(this ContainerBuilder builder, Type stateImplementation, Type stateContract)
        {
            RegisterState(builder, stateImplementation, stateContract, Lifetime.Transient);
        }

        public static void RegisterState(
            this ContainerBuilder builder,
            Type stateImplementation,
            Type stateContract,
            Lifetime lifetime)
        {
            ValidateStateBindingInput(stateImplementation, stateContract);

            builder.RegisterType(
                stateImplementation,
                new[] { stateContract },
                lifetime,
                Resolution.Lazy);
        }

        [Obsolete("Use RegisterStateMachine(builder, implementation, contract) or the overload with Lifetime.")]
        public static void AddStateMachine(
            this ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract) =>
            RegisterStateMachine(builder, stateMachineImplementation, stateMachineContract);

        [Obsolete("Use RegisterStateMachine(builder, implementation, contract, Lifetime.Singleton) instead.")]
        public static void AddSingletonStateMachine(
            this ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract) =>
            RegisterStateMachine(builder, stateMachineImplementation, stateMachineContract, Lifetime.Singleton);

        [Obsolete("Use RegisterState(builder, state) or the overload with Lifetime.")]
        public static void AddState(this ContainerBuilder builder, Type state) =>
            RegisterState(builder, state);

        [Obsolete("Use RegisterState(builder, implementation, contract) or the overload with Lifetime.")]
        public static void AddState(this ContainerBuilder builder, Type stateImplementation, Type stateContract) =>
            RegisterState(builder, stateImplementation, stateContract);

        [Obsolete("Use RegisterState(builder, state, Lifetime.Singleton) instead.")]
        public static void AddSingletonState(this ContainerBuilder builder, Type state) =>
            RegisterState(builder, state, Lifetime.Singleton);

        [Obsolete("Use RegisterState(builder, implementation, contract, Lifetime.Singleton) instead.")]
        public static void AddSingletonState(
            this ContainerBuilder builder,
            Type stateImplementation,
            Type stateContract) =>
            RegisterState(builder, stateImplementation, stateContract, Lifetime.Singleton);

        private static void ValidateStateBindingInput(Type stateImplementation, Type stateContract)
        {
            ValidateStateBindingInput(stateImplementation);

            if (!stateContract.IsAssignableFrom(stateImplementation))
            {
                throw new ArgumentException(
                    $"RegisterState({stateImplementation.Name}): Type parameter state must implement {stateContract.Name}.");
            }
        }

        private static void ValidateStateBindingInput(Type state)
        {
            if (!typeof(IExecutableState).IsAssignableFrom(state))
            {
                throw new ArgumentException(
                    $"RegisterState({state.Name}): Type parameter state must implement IState<TPayload>");
            }
        }

        private static Type[] GetStateContracts(Type state)
        {
            var interfaces = state.GetInterfaces();
            var contracts = new Type[interfaces.Length + 1];
            contracts[0] = state;

            for (var i = 0; i < interfaces.Length; i++)
            {
                contracts[i + 1] = interfaces[i];
            }

            return contracts;
        }

        private static void ValidateStateMachineBindingInput(Type stateMachineImplementation, Type stateMachineContract)
        {
            if (stateMachineImplementation == stateMachineContract)
            {
                throw new ArgumentException(
                    $"RegisterStateMachine<{stateMachineImplementation.Name}>: Type parameters must differ: " +
                    "use RegisterStateMachine() where stateMachineImplementation implements stateMachineContract.");
            }

            if (!stateMachineContract.IsAssignableFrom(stateMachineImplementation))
            {
                throw new ArgumentException(
                    $"RegisterStateMachine: Type {stateMachineImplementation.Name} " +
                    $"must implement {stateMachineContract.Name}.");
            }

            if (!typeof(IStateMachine).IsAssignableFrom(stateMachineContract))
            {
                throw new ArgumentException(
                    $"RegisterStateMachine: Type {stateMachineContract.Name} " +
                    $"must implement IStateMachine.");
            }
        }

        private static void RegisterStateMachineInternal(
            ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract,
            Lifetime lifetime)
        {
            ValidateStateMachineBindingInput(stateMachineImplementation, stateMachineContract);

            builder.RegisterFactory(
                container =>
                {
                    var stateMachine = (IStateMachine)container.Construct(stateMachineImplementation);
                    stateMachine.SetResolver(container.ToTypeResolver());
                    return stateMachine;
                },
                stateMachineImplementation,
                new[] { stateMachineImplementation, stateMachineContract },
                lifetime,
                Resolution.Lazy);
        }
    }
}

#endif

#if UNISTATE_REFLEX_SUPPORT

using System;
using Reflex.Core;

namespace UniState
{
    public static class ReflexBuildExtensions
    {
        public static void AddStateMachine(
            this ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract)
        {
            ValidateStateMachineBindingInput(stateMachineImplementation, stateMachineContract);

            builder.AddTransient(stateMachineImplementation);
            builder.AddTransient(container =>
            {
                var stateMachine = (IStateMachine)container.Resolve(stateMachineImplementation);
                stateMachine.SetResolver(container.ToTypeResolver());

                return stateMachine;
            }, stateMachineContract);
        }

        public static void AddSingletonStateMachine(
            this ContainerBuilder builder,
            Type stateMachineImplementation,
            Type stateMachineContract)
        {
            ValidateStateMachineBindingInput(stateMachineImplementation, stateMachineContract);

            builder.AddSingleton(stateMachineImplementation);
            builder.AddSingleton(container =>
            {
                var stateMachine = (IStateMachine)container.Resolve(stateMachineImplementation);
                stateMachine.SetResolver(container.ToTypeResolver());

                return stateMachine;
            }, stateMachineContract);
        }

        public static void AddState(this ContainerBuilder builder, Type state)
        {
            ValidateStateBindingInput(state);

            builder.AddTransient(state);
        }

        public static void AddState(this ContainerBuilder builder, Type stateImplementation, Type stateContract)
        {
            ValidateStateBindingInput(stateImplementation, stateContract);

            builder.AddTransient(stateImplementation, stateContract);
        }

        public static void AddSingletonState(this ContainerBuilder builder, Type state)
        {
            ValidateStateBindingInput(state);

            builder.AddSingleton(state);
        }

        public static void AddSingletonState(this ContainerBuilder builder, Type stateImplementation,
            Type stateContract)
        {
            ValidateStateBindingInput(stateImplementation, stateContract);

            builder.AddSingleton(stateImplementation, stateContract);
        }

        private static void ValidateStateBindingInput(Type stateImplementation, Type stateContract)
        {
            ValidateStateBindingInput(stateImplementation);

            if (!stateContract.IsAssignableFrom(stateImplementation))
            {
                throw new ArgumentException(
                    $"AddState({stateImplementation.Name}): Type parameter state must implement {stateContract.Name}.");
            }
        }

        private static void ValidateStateBindingInput(Type state)
        {
            if (!typeof(IExecutableState).IsAssignableFrom(state))
            {
                throw new ArgumentException(
                    $"AddState({state.Name}): Type parameter state must implement IState<TPayload>");
            }
        }

        private static void ValidateStateMachineBindingInput(Type stateMachineImplementation, Type stateMachineContract)
        {
            if (stateMachineImplementation == stateMachineContract)
            {
                throw new ArgumentException(
                    $"AddStateMachine<{stateMachineImplementation.Name}>: Type parameters must differ : " +
                    "use AddStateMachine() where stateMachineImplementation implements stateMachineContract.\");");
            }

            if (!stateMachineContract.IsAssignableFrom(stateMachineImplementation))
            {
                throw new ArgumentException(
                    $"AddStateMachine: Type {stateMachineImplementation.Name} " +
                    $"must implement {stateMachineContract.Name}.");
            }

            if (!typeof(IStateMachine).IsAssignableFrom(stateMachineContract))
            {
                throw new ArgumentException(
                    $"AddStateMachine: Type {stateMachineContract.Name} " +
                    $"must implement IStateMachine.");
            }
        }
    }
}

#endif
using System;
using System.Collections.Generic;

namespace UniState
{
    public class StateTransitionFactory : IStateTransitionFactory
    {
        private static readonly Dictionary<Type, StateBehaviourData> BehaviourDataCache = new();

        private readonly ITypeResolver _resolver;
        private readonly IStateTransitionFacade _transitionFacade;
        private readonly StateMachine _ownerMachine;

        public StateTransitionFactory(ITypeResolver resolver, StateMachine ownerMachine = null)
        {
            _resolver = resolver;
            _ownerMachine = ownerMachine;
            _transitionFacade = new StateTransitionFacade(this);
        }

        public StateTransitionInfo CreateStateTransition<TState, TPayload>(TPayload payload)
            where TState : class, IState<TPayload>
        {
            var factory = new StateFactory<TState, TPayload>(_resolver, _ownerMachine);

            factory.Setup(payload, _transitionFacade);

            return new StateTransitionInfo()
            {
                Creator = factory,
                Transition = TransitionType.State,
                StateBehaviourData = BuildStateBehaviourData(typeof(TState))
            };
        }

        public StateTransitionInfo CreateStateTransition<TState>()
            where TState : class, IState<EmptyPayload>
        {
            var factory = new StateFactory<TState, EmptyPayload>(_resolver, _ownerMachine);

            factory.Setup(EmptyPayload.Instance, _transitionFacade);

            return new StateTransitionInfo()
            {
                Creator = factory,
                Transition = TransitionType.State,
                StateBehaviourData = BuildStateBehaviourData(typeof(TState))
            };
        }

        public StateTransitionInfo CreateBackTransition() => new() { Transition = TransitionType.Back };

        public StateTransitionInfo CreateBackToTransition<TState>()
            where TState : class, IExecutableState
            => new()
            {
                Transition = TransitionType.Back,
                GoBackToType = typeof(TState),
            };

        public StateTransitionInfo CreateExitTransition() => new() { Transition = TransitionType.Exit };

        private StateBehaviourData BuildStateBehaviourData(Type stateType)
        {
            if (BehaviourDataCache.TryGetValue(stateType, out var cachedData))
            {
                return cachedData;
            }

            var data = new StateBehaviourData();

            var attribute =
                (StateBehaviourAttribute)Attribute.GetCustomAttribute(stateType, typeof(StateBehaviourAttribute));

            if (attribute != null)
            {
                data.ProhibitReturnToState = attribute.ProhibitReturnToState;
                data.InitializeOnStateTransition = attribute.InitializeOnStateTransition;
            }

            BehaviourDataCache[stateType] = data;

            return data;
        }
    }
}

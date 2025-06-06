using System;

namespace UniState
{
    public class StateTransitionFactory : IStateTransitionFactory
    {
        private readonly ITypeResolver _resolver;
        private readonly IStateTransitionFacade _transitionFacade;

        public StateTransitionFactory(ITypeResolver resolver)
        {
            _resolver = resolver;
            _transitionFacade = new StateTransitionFacade(this);
        }

        public StateTransitionInfo CreateStateTransition<TState, TPayload>(TPayload payload)
            where TState : class, IState<TPayload>
        {
            var factory = new StateFactory<TState, TPayload>(_resolver);

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
            var factory = new StateFactory<TState, EmptyPayload>(_resolver);

            factory.Setup(new EmptyPayload(), _transitionFacade);

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
            var data = new StateBehaviourData();

            var attribute =
                (StateBehaviourAttribute)Attribute.GetCustomAttribute(stateType, typeof(StateBehaviourAttribute));

            if (attribute != null)
            {
                data.ProhibitReturnToState = attribute.ProhibitReturnToState;
                data.InitializeOnStateTransition = attribute.InitializeOnStateTransition;
            }

            return data;
        }
    }
}
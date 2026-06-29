using System;
using System.Collections.Generic;

namespace UniState
{
    public class StateFactory<TState, TPayload> : IStateCreator where TState : class, IState<TPayload>
    {
        private readonly ITypeResolver _resolver;
        private readonly StateMachine _ownerMachine;

        private TPayload _payload;
        private IStateTransitionFacade _transitionFacade;

        public Type StateType => typeof(TState);

        public StateFactory(ITypeResolver resolver, StateMachine ownerMachine = null)
        {
            _resolver = resolver;
            _ownerMachine = ownerMachine;
        }

        public void Setup(TPayload payload, IStateTransitionFacade transitionFacade)
        {
            _payload = payload;
            _transitionFacade = transitionFacade;
        }

        public IExecutableState Create()
        {
            var state = _resolver.Resolve<TState>();

#if UNISTATE_DEBUG_TREE
            UniStateDebugRegistry.StateCreated(
                _ownerMachine,
                state,
                typeof(TState),
                state.GetType(),
                typeof(TPayload),
                state is ICompositeState<TPayload> ? DebugStateRole.CompositeState : DebugStateRole.State);
#endif

            if (state is ICompositeState<TPayload> compositeState)
            {
                var subStatesLinked = _resolver.Resolve<IEnumerable<ISubState<TState, TPayload>>>();
#if UNISTATE_DEBUG_TREE
                var subStatesList = new List<ISubState<TState, TPayload>>();
                var debugSubStates = new List<IExecutableState>();

                foreach (var subState in subStatesLinked)
                {
                    subStatesList.Add(subState);
                    debugSubStates.Add(subState);
                }

                UniStateDebugRegistry.CompositeSubStatesCreated(
                    _ownerMachine,
                    state,
                    typeof(TPayload),
                    debugSubStates);

                compositeState.SetSubStates(subStatesList);
#else
                compositeState.SetSubStates(subStatesLinked);
#endif
            }

            state.SetPayload(_payload);
            state.SetTransitionFacade(_transitionFacade);

            return state;
        }
    }

    public sealed class EmptyPayload
    {
        public static readonly EmptyPayload Instance = new();
    }
}

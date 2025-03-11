using System.Collections.Generic;

namespace UniState
{
    public class StateFactory<TState, TPayload> : IStateCreator where TState : class, IState<TPayload>
    {
        private readonly ITypeResolver _resolver;

        private TPayload _payload;
        private IStateTransitionFacade _transitionFacade;
        private IStateMachineFactory _stateMachineFactory;

        public StateFactory(ITypeResolver resolver)
        {
            _resolver = resolver;
        }

        public void Setup(TPayload payload, IStateTransitionFacade transitionFacade,
            IStateMachineFactory stateMachineFactory)
        {
            _payload = payload;
            _transitionFacade = transitionFacade;
            _stateMachineFactory = stateMachineFactory;
        }

        public IExecutableState Create()
        {
            var state = _resolver.Resolve<TState>();

            if (state is ICompositeState<TPayload> compositeState)
            {
                var subStatesLinked = _resolver.Resolve<IEnumerable<ISubState<TState, TPayload>>>();
                compositeState.SetSubStates(subStatesLinked);
            }

            state.SetPayload(_payload);
            state.SetTransitionFacade(_transitionFacade);
            state.SetStateMachineFactory(_stateMachineFactory);

            return state;
        }
    }

    public sealed class EmptyPayload
    {
    }
}
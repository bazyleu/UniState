namespace UniState
{
    public class StateTransitionFacade : IStateTransitionFacade
    {
        private readonly IStateTransitionFactory _factory;

        public StateTransitionFacade(IStateTransitionFactory factory)
        {
            _factory = factory;
        }

        public StateTransitionInfo GoTo<TState, TPayload>(TPayload payload)
            where TState : class, IState<TPayload> =>
            _factory.CreateStateTransition<TState, TPayload>(payload);

        public StateTransitionInfo GoTo<TState>() where TState : class, IState<EmptyPayload> =>
            _factory.CreateStateTransition<TState>();

        public StateTransitionInfo GoBack() => _factory.CreateBackTransition();

        public StateTransitionInfo GoToExit() => _factory.CreateExitTransition();
    }
}
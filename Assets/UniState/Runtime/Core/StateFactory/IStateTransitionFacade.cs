namespace UniState
{
    public interface IStateTransitionFacade
    {
        StateTransitionInfo GoTo<TState, TPayload>(TPayload payload)
            where TState : class, IState<TPayload>;

        StateTransitionInfo GoTo<TState>()
            where TState : class, IState<EmptyPayload>;

        StateTransitionInfo GoBack();
        StateTransitionInfo GoToExit();
    }
}
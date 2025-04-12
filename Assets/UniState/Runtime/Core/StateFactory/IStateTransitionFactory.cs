namespace UniState
{
    public interface IStateTransitionFactory
    {
        StateTransitionInfo CreateStateTransition<TState, TPayload>(TPayload payload)
            where TState : class, IState<TPayload>;

        StateTransitionInfo CreateStateTransition<TState>()
            where TState : class, IState<EmptyPayload>;

        StateTransitionInfo CreateBackTransition();
        StateTransitionInfo CreateBackToTransition<TState>()
            where TState : class, IExecutableState;
        StateTransitionInfo CreateExitTransition();
    }
}
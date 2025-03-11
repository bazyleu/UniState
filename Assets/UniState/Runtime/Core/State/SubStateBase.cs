namespace UniState
{
    public abstract class SubStateBase<TState, TPayload>: StateBase<TPayload>, ISubState<TState, TPayload> where TState : IState<TPayload>
    {
    }

    public abstract class SubStateBase<TState>: StateBase<EmptyPayload>, ISubState<TState, EmptyPayload> where TState : IState<EmptyPayload>
    {
    }
}
namespace UniState
{
    public interface ISubState<TState, TPayload>: IState<TPayload> where TState : IState<TPayload>
    {

    }
}
namespace UniState
{
    public interface IState<TPayload> : IExecutableState, ISetupable<TPayload>
    {

    }
}
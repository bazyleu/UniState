namespace UniState
{
    public interface ISetupable<TPayload> : IPayloadSetter<TPayload>, ITransitionFacadeSetter,
        IStateMachineFactorySetter
    {
    }
}
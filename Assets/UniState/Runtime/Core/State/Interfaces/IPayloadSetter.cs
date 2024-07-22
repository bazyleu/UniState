namespace UniState
{
    public interface IPayloadSetter<in T>
    {
        void SetPayload(T payload);
    }
}
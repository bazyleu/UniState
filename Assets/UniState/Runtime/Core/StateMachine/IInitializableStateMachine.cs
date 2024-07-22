namespace UniState
{
    public interface IInitializableStateMachine
    {
        void Initialize(ITypeResolver resolver);
    }
}
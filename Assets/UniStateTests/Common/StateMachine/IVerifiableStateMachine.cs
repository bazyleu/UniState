using UniState;

namespace UniStateTests.Common
{
    public interface IVerifiableStateMachine : IStateMachine
    {
        public void Verify();
    }
}
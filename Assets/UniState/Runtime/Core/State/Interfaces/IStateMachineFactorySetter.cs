using UniState;

namespace UniState
{
    public interface IStateMachineFactorySetter
    {
        void SetTransitionFacade(IStateTransitionFacade transitionFacade);
    }
}
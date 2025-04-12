using System;

namespace UniState
{
    public class StateBehaviourData
    {
        public Type StateType { get; }
        public bool InitializeOnStateTransition { get; set; }
        public bool ProhibitReturnToState { get; set; }

        public StateBehaviourData(Type stateType)
        {
            StateType = stateType;
        }
    }
}
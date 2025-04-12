using System;

namespace UniState
{
    public class StateTransitionInfo
    {
        public IStateCreator Creator { get; set; }
        public TransitionType Transition { get; set; }
        public StateBehaviourData StateBehaviourData { get; set; }
        public Func<StateTransitionInfo, bool> HistorySelector { get; set; }
    }
}
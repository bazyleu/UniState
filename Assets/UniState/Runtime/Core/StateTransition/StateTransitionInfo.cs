namespace UniState
{
    public class StateTransitionInfo
    {
        public IStateCreator Creator { get; set; }
        public TransitionType Transition { get; set; }
        public StateBehaviourData StateBehaviourData { get; set; }
    }
}
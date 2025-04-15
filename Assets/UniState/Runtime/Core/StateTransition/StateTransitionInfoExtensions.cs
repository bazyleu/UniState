namespace UniState
{
    public static class StateTransitionInfoExtensions
    {
        public static bool CanBeAddedToHistory(this StateTransitionInfo info)
            => info != null && info.StateBehaviourData?.ProhibitReturnToState != true;
    }
}
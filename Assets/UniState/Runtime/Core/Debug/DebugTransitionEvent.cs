namespace UniState
{
    public sealed class DebugTransitionEvent
    {
        public long Sequence { get; set; }
        public TransitionType Transition { get; set; }
        public int? FromStateId { get; set; }
        public int? ToStateId { get; set; }
        public string FromStateType { get; set; }
        public string ToStateType { get; set; }
        public string GoBackToType { get; set; }
    }
}

namespace UniState
{
    public sealed class DebugErrorInfo
    {
        public long Sequence { get; set; }
        public string ErrorType { get; set; }
        public string ExceptionType { get; set; }
        public string Message { get; set; }
        public int? StateId { get; set; }
        public string StateType { get; set; }
        public DebugStatePhase Phase { get; set; }
    }
}

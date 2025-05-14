using System;

namespace UniState
{
    public sealed class NoSubStatesException : InvalidOperationException
    {
        public NoSubStatesException()
            : base("No sub-states available for execution.") { }

        public NoSubStatesException(string message)
            : base(message) { }

        public NoSubStatesException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
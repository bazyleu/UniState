using System;

namespace UniState
{
    public sealed class AlreadyExecutingException : InvalidOperationException
    {
        public AlreadyExecutingException()
            : base("The state machine is already running.") { }

        public AlreadyExecutingException(string message)
            : base(message) { }

        public AlreadyExecutingException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
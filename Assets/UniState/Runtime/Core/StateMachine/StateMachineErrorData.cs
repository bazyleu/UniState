using System;

namespace UniState
{
    public class StateMachineErrorData
    {
        public Exception Exception { get; }
        public StateMachineErrorType ErrorType { get; }
        public IExecutableState State { get; }

        public StateMachineErrorData(Exception exception, StateMachineErrorType errorType, IExecutableState state = null)
        {
            Exception = exception;
            ErrorType = errorType;
            State = state;
        }
    }
}
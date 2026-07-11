using System;

namespace UniState
{
    public sealed class NoSubStatesException : InvalidOperationException
    {
        public NoSubStatesException()
            : base("No sub-states available for execution. Sub-states are resolved by the state type used in the " +
                   "transition, so a composite state opened via GoTo<TState>() only finds sub-states declared as " +
                   "SubStateBase<TState> (or bound as ISubState<TState, TPayload>) for that exact TState.") { }

        public NoSubStatesException(string message)
            : base(message) { }

        public NoSubStatesException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
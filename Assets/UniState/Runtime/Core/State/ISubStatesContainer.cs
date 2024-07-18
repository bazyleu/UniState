using System.Collections.Generic;

namespace UniState
{
    public interface ISubStatesContainer<TPayload> : IExecutableState
    {
        List<IState<TPayload>> List { get; }
    }
}
using System.Collections.Generic;

namespace UniState
{
    public interface ICompositeState<TPayload> : IState<TPayload>
    {
        public void SetSubStates<T>(IEnumerable<ISubState<T, TPayload>> subStates) where T : IState<TPayload>;
    }
}
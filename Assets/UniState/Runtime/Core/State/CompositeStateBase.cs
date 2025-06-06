using System.Collections.Generic;

namespace UniState
{
    public abstract class CompositeStateBase : CompositeStateBase<EmptyPayload>
    {
    }

    public abstract class CompositeStateBase<TPayload> : StateBase<TPayload>, ICompositeState<TPayload>
    {
        private readonly SubStatesContainer<TPayload> _subStatesContainer = new();

        protected ISubStatesContainer<TPayload> SubStates => _subStatesContainer;

        public void SetSubStates<T>(IEnumerable<ISubState<T, TPayload>> subStates) where T : IState<TPayload>
        {
            List<IState<TPayload>> subStatesList = new();

            foreach (var subState in subStates)
            {
                subStatesList.Add(subState);
            }

            _subStatesContainer.Initialize(subStatesList);
        }

        public override void SetPayload(TPayload payload)
        {
            base.SetPayload(payload);
            _subStatesContainer.SetPayload(payload);
        }

        public override void SetTransitionFacade(IStateTransitionFacade transitionFacade)
        {
            base.SetTransitionFacade(transitionFacade);
            _subStatesContainer.SetTransitionFacade(transitionFacade);
        }

        public override void Dispose()
        {
            base.Dispose();
            _subStatesContainer.Dispose();
        }
    }
}
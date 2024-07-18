using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

namespace UniState
{
    public abstract class StateBase : StateBase<EmptyPayload>
    {
    }

    public abstract class StateBase<T> : IState<T>
    {
        protected T Payload { get; private set; }

        protected IStateTransitionFacade Transition { get; private set; }
        protected IStateMachineFactory StateMachineFactory { get; private set; }

        public abstract UniTask<StateTransitionInfo> Execute(CancellationToken token);

        public virtual UniTask Initialize(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask Exit(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public virtual void SetPayload(T payload)
        {
            Payload = payload;
        }

        public virtual void SetTransitionFacade(IStateTransitionFacade transitionFacade) => Transition = transitionFacade;

        public virtual void SetStateMachineFactory(IStateMachineFactory stateMachineFactory) =>
            StateMachineFactory = stateMachineFactory;

        public virtual void Dispose()
        {

        }
    }
}
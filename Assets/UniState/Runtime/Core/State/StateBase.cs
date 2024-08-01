using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState
{
    public abstract class StateBase : StateBase<EmptyPayload>
    {
    }

    public abstract class StateBase<T> : IState<T>
    {
        private List<IDisposable> _disposables;

        protected T Payload { get; private set; }

        protected IStateTransitionFacade Transition { get; private set; }
        protected IStateMachineFactory StateMachineFactory { get; private set; }
        protected List<IDisposable> Disposables => _disposables ??= new(4);

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
            if (_disposables?.Count > 0)
            {
                for (var i = _disposables.Count - 1; i >= 0; i--)
                {
                    var disposable = _disposables[i];
                    disposable?.Dispose();
                }

                _disposables = null;
            }
        }
    }
}
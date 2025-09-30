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
        protected List<IDisposable> Disposables => _disposables ??= new(4);

        public abstract UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token);

        public virtual UniTask InitializeAsync(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask ExitAsync(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public virtual void SetPayload(T payload)
        {
            Payload = payload;
        }

        public virtual void SetTransitionFacade(IStateTransitionFacade transitionFacade) => Transition = transitionFacade;

        public virtual void Dispose()
        {
            _disposables.Dispose();
            _disposables = null;
        }
    }
}
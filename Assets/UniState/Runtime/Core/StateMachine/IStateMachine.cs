using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState
{
    public interface IStateMachine
    {
        bool IsExecuting { get; }

        UniTask ExecuteAsync<TState>(CancellationToken token) where TState : class, IState<EmptyPayload>;
        UniTask ExecuteAsync<TState, TPayload>(TPayload payload, CancellationToken token)
            where TState : class, IState<TPayload>;
        void SetResolver(ITypeResolver resolver);
    }
}
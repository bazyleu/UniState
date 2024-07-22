using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

namespace UniState
{
    public interface IExecutableStateMachine
    {
        UniTask Execute<TState>(CancellationToken token) where TState : class, IState<EmptyPayload>;

        UniTask Execute<TState, TPayload>(TPayload payload, CancellationToken token)
            where TState : class, IState<TPayload>;
    }
}
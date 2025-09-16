using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState
{
    public interface IExecutableState : IDisposable
    {
        UniTask InitializeAsync(CancellationToken token);
        UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token);
        UniTask ExitAsync(CancellationToken token);
    }
}
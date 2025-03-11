using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState
{
    public interface IExecutableState : IDisposable
    {
        UniTask Initialize(CancellationToken token);
        UniTask<StateTransitionInfo> Execute(CancellationToken token);
        UniTask Exit(CancellationToken token);
    }
}
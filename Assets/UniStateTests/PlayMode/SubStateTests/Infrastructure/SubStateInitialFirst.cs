using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.SubStateTests.Infrastructure
{
    internal class SubStateInitialFirst : SubStateBase<StateInitial>
    {
        private readonly ExecutionLogger _logger;

        public SubStateInitialFirst(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            Disposables.Add(() => { _logger.LogStep("SubStateInitialFirst", "Disposables"); });

            await UniTask.Yield(token);

            _logger.LogStep("SubStateInitialFirst", "Execute");

            await UniTask.Yield(token);
            await UniTask.Yield(token);
            await UniTask.Yield(token);

            return Transition.GoToExit();
        }
    }
}
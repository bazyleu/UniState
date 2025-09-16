using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.SubStateTests.Infrastructure
{
    internal class SubStateInitialSecond: SubStateBase<StateInitial>
    {
        private readonly ExecutionLogger _logger;

        public SubStateInitialSecond(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            Disposables.Add(() => { _logger.LogStep("SubStateInitialSecond", "Disposables"); });

            _logger.LogStep("SubStateInitialSecond", "Execute");

            await UniTask.Yield(token);
            await UniTask.Yield(token);

            return Transition.GoTo<StateFinal>();
        }
    }
}
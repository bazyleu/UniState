using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.SubStateTests.Infrastructure
{
    internal class SubStateFinalSecond : SubStateBase<StateFinal>
    {
        private readonly ExecutionLogger _logger;

        public SubStateFinalSecond(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            Disposables.Add(() => { _logger.LogStep("SubStateFinalSecond", "Disposables"); });

            _logger.LogStep("SubStateFinalSecond", "Execute");

            await UniTask.Yield(token);
            await UniTask.Yield(token);

            throw new Exception("SubStateFinalSecond exception");
        }
    }
}
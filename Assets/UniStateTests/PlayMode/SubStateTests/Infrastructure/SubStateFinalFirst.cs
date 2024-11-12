using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.SubStateTests.Infrastructure
{
    internal class SubStateFinalFirst : SubStateBase<StateFinal>
    {
        private readonly ExecutionLogger _logger;

        public SubStateFinalFirst(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Disposables.Add(() => { _logger.LogStep("SubStateFinalFirst", "Disposables"); });

            await UniTask.Yield(token);

            _logger.LogStep("SubStateFinalFirst", "Execute");

            await UniTask.Yield(token);
            await UniTask.Yield(token);
            await UniTask.Yield(token);
            await UniTask.Yield(token);

            return Transition.GoBack();
        }
    }
}
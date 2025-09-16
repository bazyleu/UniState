using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackToTests.Infrastructure
{
    internal class GoBackToState4 : StateBase
    {
        private readonly ExecutionLogger _logger;

        public GoBackToState4(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep(nameof(GoBackToState4), "Execute");

            return UniTask.FromResult(Transition.GoBackTo<GoBackToState2>());
        }
    }
}
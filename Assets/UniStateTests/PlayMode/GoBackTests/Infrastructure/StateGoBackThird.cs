using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackTests.Infrastructure
{
    internal class StateGoBackThird : StateBase
    {
        private readonly ExecutionLogger _logger;
        public StateGoBackThird(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep("StateGoBackThird", "Execute");

            await UniTask.Yield();

            return Transition.GoBack();
        }
    }
}
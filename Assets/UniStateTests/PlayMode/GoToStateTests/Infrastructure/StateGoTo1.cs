using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal class StateGoTo1: StateBase
    {
        private readonly ExecutionLogger _logger;
        public StateGoTo1(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep("StateGoTo1", "Execute");

            return UniTask.FromResult(Transition.GoTo<StateGoTo2>());
        }
    }
}
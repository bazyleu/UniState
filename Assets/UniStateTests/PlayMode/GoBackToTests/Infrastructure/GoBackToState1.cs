using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackToTests.Infrastructure
{
    internal class GoBackToState1 : StateBase
    {
        private readonly ExecutionLogger _logger;

        public GoBackToState1(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep(nameof(GoBackToState1), "Execute");

            return UniTask.FromResult(Transition.GoTo<GoBackToState2, int>(42));
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackToTests.Infrastructure
{
    internal class GoBackToState2 : StateBase<int>
    {
        private readonly ExecutionLogger _logger;
        private readonly GoBackToTestsHelper _helper;

        public GoBackToState2(ExecutionLogger logger, GoBackToTestsHelper helper)
        {
            _logger = logger;
            _helper = helper;
        }

        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep(nameof(GoBackToState2), $"Execute:{Payload}");
            
            var result = _helper.SecondStateExecuted
                ? Transition.GoBackTo<GoBackToState3>()
                : Transition.GoTo<GoBackToState3>();

            _helper.SecondStateExecuted = true;
            
            return UniTask.FromResult(result);
        }
    }
}
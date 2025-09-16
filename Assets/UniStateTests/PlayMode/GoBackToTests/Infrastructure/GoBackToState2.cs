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

        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep(nameof(GoBackToState2), $"Execute:{Payload}");
            
            var result = _helper.SecondStateExecuteCount switch
            {
                0 => Transition.GoTo<GoBackToState2, int>(11),
                1 => Transition.GoTo<GoBackToState3>(),
                _ => Transition.GoBackTo<GoBackToState3>(),
            };

            _helper.SecondStateExecuteCount++;
            
            return UniTask.FromResult(result);
        }
    }
}
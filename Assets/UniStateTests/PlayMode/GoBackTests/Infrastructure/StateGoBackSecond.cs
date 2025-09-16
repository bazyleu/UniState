using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackTests.Infrastructure
{
    internal class StateGoBackSecond : StateBase<int>
    {
        private readonly ExecutionLogger _logger;
        private readonly GoBackTestHelper _goBackFlags;

        public StateGoBackSecond(ExecutionLogger logger, GoBackTestHelper goBackFlags)
        {
            _logger = logger;
            _goBackFlags = goBackFlags;
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep("StateGoBackSecond", $"Execute:{Payload}");

            await UniTask.Yield();

            if (_goBackFlags.ExecutedState2)
            {
                return Transition.GoBack();
            }

            _goBackFlags.ExecutedState2 = true;

            return Transition.GoTo<StateGoBackThird>();
        }
    }
}
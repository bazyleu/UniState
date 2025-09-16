using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackTests.Infrastructure
{
    internal class StateGoBackFirst : StateBase
    {
        private readonly ExecutionLogger _logger;
        private readonly GoBackTestHelper _goBackFlags;

        public StateGoBackFirst(ExecutionLogger logger, GoBackTestHelper goBackFlags)
        {
            _logger = logger;
            _goBackFlags = goBackFlags;
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep("StateGoBackFirst", "Execute");

            await UniTask.Yield();

            if (_goBackFlags.ExecutedState1)
            {
                return Transition.GoBack();
            }

            _goBackFlags.ExecutedState1 = true;

            return Transition.GoTo<StateGoBackSecond, int>(42);
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackTests.Infrastructure
{
    internal class StateGoBack3 : StateBase
    {
        private readonly ExecutionLogger _logger;
        public StateGoBack3(ExecutionLogger logger, GoBackFlagsData goBackFlags)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("StateGoBack3", "Execute");

            await UniTask.Yield();

            return Transition.GoBack();
        }
    }
}
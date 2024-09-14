using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackTests.Infrastructure
{
    internal class StateGoBack2 : StateBase<int>
    {
        private readonly ExecutionLogger _logger;
        private readonly GoBackFlagsData _goBackFlags;

        public StateGoBack2(ExecutionLogger logger, GoBackFlagsData goBackFlags)
        {
            _logger = logger;
            _goBackFlags = goBackFlags;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("StateGoBack2", $"Execute:{Payload}");

            await UniTask.Yield();

            if (_goBackFlags.ExecutedState2)
            {
                return Transition.GoBack();
            }

            _goBackFlags.ExecutedState2 = true;

            return Transition.GoTo<StateGoBack3>();
        }
    }
}
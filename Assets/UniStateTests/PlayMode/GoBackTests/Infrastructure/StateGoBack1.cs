using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackTests.Infrastructure
{
    internal class StateGoBack1 : StateBase
    {
        private readonly ExecutionLogger _logger;
        private readonly GoBackFlagsData _goBackFlags;

        public StateGoBack1(ExecutionLogger logger, GoBackFlagsData goBackFlags)
        {
            _logger = logger;
            _goBackFlags = goBackFlags;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("StateGoBack1", "Execute");

            await UniTask.Yield();

            if (_goBackFlags.ExecutedState1)
            {
                return Transition.GoBack();
            }

            _goBackFlags.ExecutedState1 = true;

            return Transition.GoTo<StateGoBack2>();
        }
    }
}
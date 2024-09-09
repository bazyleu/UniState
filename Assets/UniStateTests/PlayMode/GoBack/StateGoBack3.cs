using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBack
{
    public class StateGoBack3 : StateBase
    {
        private readonly ExecutionLogger _logger;
        private readonly GoBackTestFlags _flags;

        public StateGoBack3(ExecutionLogger logger, GoBackTestFlags flags)
        {
            _logger = logger;
            _flags = flags;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("StateGoBack3", "Execute");

            await UniTask.Yield();

            return Transition.GoBack();
        }
    }
}
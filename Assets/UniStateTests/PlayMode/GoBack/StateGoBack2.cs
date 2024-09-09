using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBack
{
    public class StateGoBack2 : StateBase
    {
        private readonly ExecutionLogger _logger;
        private readonly GoBackTestFlags _flags;

        public StateGoBack2(ExecutionLogger logger, GoBackTestFlags flags)
        {
            _logger = logger;
            _flags = flags;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("StateGoBack2", "Execute");

            await UniTask.Yield();

            if (_flags.ExecutedState2)
            {
                return Transition.GoBack();
            }

            _flags.ExecutedState2 = true;

            return Transition.GoTo<StateGoBack3>();
        }
    }
}
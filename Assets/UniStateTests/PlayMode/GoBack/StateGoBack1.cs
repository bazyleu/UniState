using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBack
{
    public class StateGoBack1 : StateBase
    {
        private readonly ExecutionLogger _logger;
        private readonly GoBackTestFlags _flags;

        public StateGoBack1(ExecutionLogger logger, GoBackTestFlags flags)
        {
            _logger = logger;
            _flags = flags;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("StateGoBack1", "Execute");

            await UniTask.Yield();

            if (_flags.ExecutedState1)
            {
                return Transition.GoBack();
            }

            _flags.ExecutedState1 = true;

            return Transition.GoTo<StateGoBack2>();
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure
{
    internal class StateInitial : StateBase
    {
        private readonly ExecutionLogger _logger;

        public StateInitial(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("StateInitial", $"Execute");

            return Transition.GoTo<StateThrowTwoException>();
        }
    }
}
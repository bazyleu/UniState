using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure
{
    internal class StateMagicRecovery : StateBase
    {
        private readonly ExecutionLogger _logger;

        public StateMagicRecovery(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("StateMagicRecovery", $"Execute");

            return Transition.GoToExit();
        }
    }
}
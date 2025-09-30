using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure
{
    internal class StateThrowTwoException : StateBase
    {
        private readonly ExecutionLogger _logger;

        public StateThrowTwoException(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask InitializeAsync(CancellationToken token)
        {
            _logger.LogStep("StateThrowTwoException", $"Initialize");

            throw new Exception("Initialize exception");
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("StateThrowTwoException", $"Execute");

            return Transition.GoTo<StateWithFailExecution>();
        }

        public override UniTask ExitAsync(CancellationToken token)
        {
            _logger.LogStep("StateThrowTwoException", $"Exit");

            throw new Exception("Exit exception");
        }
    }
}
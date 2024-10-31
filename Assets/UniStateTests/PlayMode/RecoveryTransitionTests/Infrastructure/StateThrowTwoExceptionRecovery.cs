using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure
{
    internal class StateThrowTwoExceptionRecovery : StateBase
    {
        private readonly ExecutionLogger _logger;

        public StateThrowTwoExceptionRecovery(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask Initialize(CancellationToken token)
        {
            _logger.LogStep("StateThrowTwoExceptionRecovery", $"Initialize");

            throw new Exception("Initialize exception");
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("StateThrowTwoExceptionRecovery", $"Execute");

            return Transition.GoTo<StateFailExecutionRecovery>();
        }

        public override UniTask Exit(CancellationToken token)
        {
            _logger.LogStep("StateThrowTwoExceptionRecovery", $"Exit");

            throw new Exception("Exit exception");
        }
    }
}
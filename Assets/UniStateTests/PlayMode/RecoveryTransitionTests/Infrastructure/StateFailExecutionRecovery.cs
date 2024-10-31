using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure
{
    internal class StateFailExecutionRecovery : StateBase
    {
        private readonly ExecutionLogger _logger;
        private readonly RecoveryTestHelper _testHelper;

        public StateFailExecutionRecovery(RecoveryTestHelper testHelper, ExecutionLogger logger)
        {
            _testHelper = testHelper;
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Disposables.Add(() => { _logger.LogStep("StateFailExecutionRecovery", $"Disposables"); });

            _logger.LogStep("StateFailExecutionRecovery", $"Execute");

            if (_testHelper.ExceptionWasThrown)
            {
                return UniTask.FromResult(Transition.GoToExit());
            }

            _testHelper.ExceptionWasThrown = true;

            throw new Exception("Execution exception");
        }

        public override UniTask Exit(CancellationToken token)
        {
            _logger.LogStep("StateFailExecutionRecovery", $"Exit");

            return UniTask.CompletedTask;
        }
    }
}
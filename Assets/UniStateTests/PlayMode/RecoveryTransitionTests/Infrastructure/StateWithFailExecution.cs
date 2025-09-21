using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure
{
    internal class StateWithFailExecution : StateBase
    {
        private readonly ExecutionLogger _logger;
        private readonly RecoveryTestHelper _testHelper;

        public StateWithFailExecution(RecoveryTestHelper testHelper, ExecutionLogger logger)
        {
            _testHelper = testHelper;
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            Disposables.Add(() => { _logger.LogStep("StateWithFailExecution", $"Disposables"); });

            _logger.LogStep("StateWithFailExecution", $"Execute");

            if (_testHelper.ExceptionWasThrown)
            {
                return UniTask.FromResult(Transition.GoToExit());
            }

            _testHelper.ExceptionWasThrown = true;

            throw new Exception("Execution exception");
        }

        public override UniTask ExitAsync(CancellationToken token)
        {
            _logger.LogStep("StateWithFailExecution", $"Exit");

            return UniTask.CompletedTask;
        }
    }
}
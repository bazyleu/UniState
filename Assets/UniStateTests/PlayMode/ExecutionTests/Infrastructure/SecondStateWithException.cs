using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.Execution.Infrastructure
{
    public class SecondStateWithException : StateBase
    {
        private readonly ExecutionTestHelper _machineTestHelper;
        private readonly ExecutionLogger _logger;

        public SecondStateWithException(ExecutionTestHelper machineTestHelper, ExecutionLogger logger)
        {
            _machineTestHelper = machineTestHelper;
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep("SecondStateWithException", _machineTestHelper.CurrentStateMachine.IsExecuting.ToString());

            return UniTask.FromResult(Transition.GoToExit());
        }

        public override UniTask ExitAsync(CancellationToken token)
        {
            throw new Exception("test exception");
        }
    }
}
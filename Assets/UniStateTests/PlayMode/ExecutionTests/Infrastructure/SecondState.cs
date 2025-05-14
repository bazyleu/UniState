using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.Execution.Infrastructure
{
    public class SecondState: StateBase
    {
        private readonly ExecutionTestHelper _machineTestHelper;
        private readonly ExecutionLogger _logger;

        public SecondState(ExecutionTestHelper machineTestHelper, ExecutionLogger logger)
        {
            _machineTestHelper = machineTestHelper;
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("SecondState", _machineTestHelper.CurrentStateMachine.IsExecuting.ToString());

            return UniTask.FromResult(Transition.GoToExit());
        }
    }
}
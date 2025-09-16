using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.Execution.Infrastructure
{
    public class SecondStateWithWrongDependency : StateBase
    {
        private readonly ExecutionTestHelper _machineTestHelper;
        private readonly ExecutionLogger _logger;

        public SecondStateWithWrongDependency(ExecutionTestHelper machineTestHelper, ExecutionLogger logger,
            byte wrongDependency)
        {
            _machineTestHelper = machineTestHelper;
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep("SecondStateWithWrongDependency",
                _machineTestHelper.CurrentStateMachine.IsExecuting.ToString());

            return UniTask.FromResult(Transition.GoToExit());
        }
    }
}
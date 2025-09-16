using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.Execution.Infrastructure
{
    public class FirstState : StateBase
    {
        private readonly ExecutionTestHelper _machineTestHelper;
        private readonly ExecutionLogger _logger;

        public FirstState(ExecutionTestHelper machineTestHelper, ExecutionLogger logger)
        {
            _machineTestHelper = machineTestHelper;
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep("FirstState", _machineTestHelper.CurrentStateMachine.IsExecuting.ToString());

            switch (_machineTestHelper.ExecutionType)
            {
                case StateMachineExecutionType.Default:
                    return UniTask.FromResult(Transition.GoTo<SecondState>());
                case StateMachineExecutionType.Exception:
                    return UniTask.FromResult(Transition.GoTo<SecondStateWithException>());
                case StateMachineExecutionType.WrongDependency:
                    return UniTask.FromResult(Transition.GoTo<SecondStateWithWrongDependency>());
            }

            return UniTask.FromResult(Transition.GoToExit());
        }
    }
}
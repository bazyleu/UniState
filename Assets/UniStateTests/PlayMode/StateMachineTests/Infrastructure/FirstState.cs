using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.StateMachineTests.Infrastructure
{
    public class FirstState : StateBase
    {
        private readonly StateMachineTestHelper _machineTestHelper;
        private readonly ExecutionLogger _logger;

        public FirstState(StateMachineTestHelper machineTestHelper, ExecutionLogger logger)
        {
            _machineTestHelper = machineTestHelper;
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("FirstState", $"Execute");
            _logger.LogStep("StateMachine", _machineTestHelper.CurrentStateMachine.IsExecuting.ToString());

            switch (_machineTestHelper.ExecutionType)
            {
                case StateMachineExecutionType.Default:
                    return UniTask.FromResult(Transition.GoTo<SecondState>());
                case StateMachineExecutionType.Exception:
                    return UniTask.FromResult(Transition.GoTo<SecondStateWithException>());
                case StateMachineExecutionType.WrongDependency:
                    return UniTask.FromResult(Transition.GoTo<SecondStateWithWrongDependency>());
            }

            //TODO: Checks with infinine loop
            return UniTask.FromResult(Transition.GoToExit());
        }
    }
}
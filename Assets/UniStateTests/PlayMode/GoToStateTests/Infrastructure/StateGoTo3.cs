using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal abstract class StateGoToAbstract3 : StateBase
    {
    }

    internal class StateGoTo3 : StateGoToAbstract3
    {
        private readonly ExecutionLogger _logger;
        public StateGoTo3(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("StateGoTo3", "Execute");

            return UniTask.FromResult(Transition.GoTo<IStateGoTo4>());
        }
    }
}
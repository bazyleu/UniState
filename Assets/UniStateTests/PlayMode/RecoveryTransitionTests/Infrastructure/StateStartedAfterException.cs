using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure
{
    internal class StateStartedAfterException : StateBase
    {
        private readonly ExecutionLogger _logger;

        public StateStartedAfterException(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("StateStartedAfterException", $"Execute");

            return Transition.GoToExit();
        }
    }
}
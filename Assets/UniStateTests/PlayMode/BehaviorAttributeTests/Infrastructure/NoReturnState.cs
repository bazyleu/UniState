using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.StateBehaviorAttributeTests.Infrastructure
{
    [StateBehaviour(ProhibitReturnToState = true)]
    internal class NoReturnState : StateBase
    {
        private readonly ExecutionLogger _logger;

        public NoReturnState(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask Initialize(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("NoReturnState", $"Initialize");
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("NoReturnState", $"Execute");

            return Transition.GoTo<FastInitializeState>();
        }

        public override async UniTask Exit(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("NoReturnState", $"Exit");
        }
    }
}
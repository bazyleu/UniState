using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.StateBehaviorAttributeTests.Infrastructure
{
    [StateBehaviour(InitializeOnStateTransition = true)]
    internal class FastInitializeState : StateBase
    {
        private readonly ExecutionLogger _logger;

        public FastInitializeState(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask Initialize(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("FastInitializeState", $"Initialize");
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("FastInitializeState", $"Execute");

            return Transition.GoBack();
        }

        public override async UniTask Exit(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("FastInitializeState", $"Exit");
        }
    }
}
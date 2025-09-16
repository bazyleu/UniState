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

        public override async UniTask InitializeAsync(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("FastInitializeState", $"Initialize");
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("FastInitializeState", $"Execute");

            return Transition.GoBack();
        }

        public override async UniTask ExitAsync(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("FastInitializeState", $"Exit");
        }
    }
}
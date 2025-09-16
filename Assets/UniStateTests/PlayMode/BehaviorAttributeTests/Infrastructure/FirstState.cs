using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.StateBehaviorAttributeTests.Infrastructure
{
    internal class FirstState : StateBase
    {
        private readonly ExecutionLogger _logger;
        private readonly BehaviourAttributeTestHelper _testHelper;

        public FirstState(ExecutionLogger logger, BehaviourAttributeTestHelper testHelper)
        {
            _logger = logger;
            _testHelper = testHelper;
        }

        public override async UniTask InitializeAsync(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("FirstState", $"Initialize");
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("FirstState", $"Execute");

            if (_testHelper.ExecutedFirstState)
            {
                return Transition.GoToExit();
            }

            _testHelper.ExecutedFirstState = true;

            return Transition.GoTo<NoReturnState>();
        }

        public override async UniTask ExitAsync(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("FirstState", $"Exit");
        }
    }
}
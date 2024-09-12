using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal class CompositeStateGoTo6 : DefaultCompositeState
    {

    }


    internal class SubStateGoToX6A : SubStateBase<CompositeStateGoTo6>
    {
        private readonly ExecutionLogger _logger;

        public SubStateGoToX6A(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("SubStateGoToX6A", "Execute");

            await UniTask.Yield(token);
            await UniTask.Yield(token);

            return Transition.GoTo<CompositeStateGoTo7, CompositeStatePayload>(new CompositeStatePayload(true));
        }
    }

    internal class SubStateGoToX6B : SubStateBase<CompositeStateGoTo6>
    {
        private readonly ExecutionLogger _logger;

        public SubStateGoToX6B(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("SubStateGoToX6B", "Execute");

            await UniTask.Yield(token);
            await UniTask.Yield(token);
            await UniTask.Yield(token);

            return Transition.GoToExit();
        }
    }
}
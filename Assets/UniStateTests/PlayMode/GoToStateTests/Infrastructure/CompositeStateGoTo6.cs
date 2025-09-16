using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal class CompositeStateGoTo6 : DefaultCompositeState
    {

    }


    internal class SubStateGoTo6First : SubStateBase<CompositeStateGoTo6>
    {
        private readonly ExecutionLogger _logger;

        public SubStateGoTo6First(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep("SubStateGoTo6First", "Execute");

            await UniTask.Yield(token);
            await UniTask.Yield(token);

            return Transition.GoTo<CompositeStateGoTo7, CompositeStatePayload>(new CompositeStatePayload(true));
        }
    }

    internal class SubStateGoTo6Second : SubStateBase<CompositeStateGoTo6>
    {
        private readonly ExecutionLogger _logger;

        public SubStateGoTo6Second(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("SubStateGoTo6Second", "Execute");

            await UniTask.Yield(token);
            await UniTask.Yield(token);
            await UniTask.Yield(token);

            return Transition.GoToExit();
        }
    }
}
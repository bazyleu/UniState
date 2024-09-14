using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal class CompositeStateGoTo7: DefaultCompositeState<CompositeStatePayload>
    {

    }

    internal class SubStateGoToX7A : SubStateBase<CompositeStateGoTo7, CompositeStatePayload>
    {
        private readonly ExecutionLogger _logger;

        public SubStateGoToX7A(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("SubStateGoToX7A", $"Execute:{Payload.DelayFirstSubState}");

            if (Payload.DelayFirstSubState)
            {
                await UniTask.Yield(token);
                await UniTask.Yield(token);
                await UniTask.Yield(token);
                await UniTask.Yield(token);
                await UniTask.Yield(token);
            }

            await UniTask.Yield(token);
            await UniTask.Yield(token);
            
            return Transition.GoTo<IStateGoTo8, bool>(true);
        }
    }

    internal class SubStateGoToX7B : SubStateBase<CompositeStateGoTo7, CompositeStatePayload>
    {
        private readonly ExecutionLogger _logger;

        public SubStateGoToX7B(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("SubStateGoToX7B", $"Execute:{Payload.DelayFirstSubState}");

            await UniTask.Yield(token);
            await UniTask.Yield(token);

            return Transition.GoTo<StateGoTo8, bool>(false);
        }
    }

    internal class CompositeStatePayload
    {
        public bool DelayFirstSubState { get; }

        public CompositeStatePayload(bool delayFirstSubState)
        {
            DelayFirstSubState = delayFirstSubState;
        }
    }
}
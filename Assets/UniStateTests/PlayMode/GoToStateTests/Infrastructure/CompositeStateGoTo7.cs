using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal class CompositeStateGoTo7: DefaultCompositeState<CompositeStatePayload>
    {

    }

    internal class SubStateGoTo7First : SubStateBase<CompositeStateGoTo7, CompositeStatePayload>
    {
        private readonly ExecutionLogger _logger;

        public SubStateGoTo7First(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep("SubStateGoTo7First", $"Execute:{Payload.DelayFirstSubState}");

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

    internal class SubStateGoTo7Second : SubStateBase<CompositeStateGoTo7, CompositeStatePayload>
    {
        private readonly ExecutionLogger _logger;

        public SubStateGoTo7Second(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("SubStateGoTo7Second", $"Execute:{Payload.DelayFirstSubState}");

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
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal class StateGoTo5 : StateBase<StateGoTo5Payload>
    {
        private readonly ExecutionLogger _logger;

        public StateGoTo5(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep("StateGoTo5", $"Execute:{Payload.Value}");

            return UniTask.FromResult(Transition.GoTo<CompositeStateGoTo6>());
        }
    }

    internal class StateGoTo5Payload
    {
        public int Value { get; }

        public StateGoTo5Payload(int value)
        {
            Value = value;
        }
    }
}
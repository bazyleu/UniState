using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal interface IStateGoTo8 : IState<bool>
    {
    }

    internal class StateGoTo8 : StateBase<bool>, IStateGoTo8
    {
        private readonly ExecutionLogger _logger;

        public StateGoTo8(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("StateGoTo8", $"Execute:{Payload}");

            if (Payload)
            {
                return UniTask.FromResult(Transition.GoToExit());
            }

            return UniTask.FromResult(Transition.GoTo<CompositeStateGoTo7, CompositeStatePayload>(new CompositeStatePayload(false)));
        }
    }
}
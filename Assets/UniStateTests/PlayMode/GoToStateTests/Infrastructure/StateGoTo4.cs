using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;
using UnityEngine;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal interface IStateGoTo4 : IState<EmptyPayload>
    {
    }

    internal class StateGoTo4 : StateBase, IStateGoTo4
    {
        private readonly ExecutionLogger _logger;
        public StateGoTo4(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep("StateGoTo4", "Execute");

            return UniTask.FromResult(Transition.GoTo<StateGoTo5, StateGoTo5Payload>(new StateGoTo5Payload(42)));
        }
    }
}
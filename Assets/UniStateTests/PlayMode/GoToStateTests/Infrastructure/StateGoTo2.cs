using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;
using UnityEngine;

namespace UniStateTests.PlayMode.GoToStateTests.Infrastructure
{
    internal class StateGoTo2 : StateBase
    {
        private readonly ExecutionLogger _logger;
        public StateGoTo2(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _logger.LogStep("StateGoTo2", "Execute");

            return UniTask.FromResult(Transition.GoTo<StateGoToAbstract3>());
        }
    }
}
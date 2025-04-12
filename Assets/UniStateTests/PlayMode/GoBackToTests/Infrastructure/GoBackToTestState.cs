using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.GoBackToTests.Infrastructure
{
    internal class GoBackToTestState<T> : StateBase
    {
        private readonly GoBackToTestsHelper _goBackToTestsHelper;
        private readonly ExecutionLogger _logger;

        public GoBackToTestState(
            GoBackToTestsHelper goBackToTestsHelper,
            ExecutionLogger logger)
        {
            _goBackToTestsHelper = goBackToTestsHelper;
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            var info = _goBackToTestsHelper.TransitionsStack.Pop();
            _logger.LogStep(typeof(T).Name, info.Log);

            return UniTask.FromResult(info.Transition);
        }
    }
}
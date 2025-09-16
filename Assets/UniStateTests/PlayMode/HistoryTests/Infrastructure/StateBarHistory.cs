using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.HistoryTests.Infrastructure
{
    internal class StateBarHistory : StateBase
    {
        private readonly HistorySizeTestHelper _testHelper;
        private readonly ExecutionLogger _logger;

        public StateBarHistory(HistorySizeTestHelper testHelper, ExecutionLogger logger)
        {
            _testHelper = testHelper;
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            await UniTask.Yield(token);

            _testHelper.ReportTransition();

            _logger.LogStep("StateBarHistory", $"{_testHelper.ShouldGoBack}");

            return _testHelper.ShouldGoBack ? Transition.GoBack() : Transition.GoTo<StateFooHistory>();
        }
    }
}
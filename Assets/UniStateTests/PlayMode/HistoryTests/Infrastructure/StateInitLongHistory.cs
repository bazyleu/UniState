using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.HistoryTests.Infrastructure
{
    internal class StateInitLongHistory : StateBase
    {
        private readonly HistorySizeTestHelper _testHelper;
        private readonly ExecutionLogger _logger;

        public StateInitLongHistory(HistorySizeTestHelper testHelper, ExecutionLogger logger)
        {
            _testHelper = testHelper;
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await UniTask.Yield(token);

            _testHelper.SetMaxTransitionCount(StateMachineLongHistory.MaxTransition);

            _logger.LogStep("StateInitLongHistory", $"Execute");

            return Transition.GoTo<StateFooHistory>();
        }
    }
}
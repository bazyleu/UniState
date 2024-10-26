using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.HistoryTests.Infrastructure
{
    internal class StateInitZeroHistory : StateBase
    {
        private readonly HistorySizeTestHelper _testHelper;
        private readonly ExecutionLogger _logger;

        public StateInitZeroHistory(HistorySizeTestHelper testHelper, ExecutionLogger logger)
        {
            _testHelper = testHelper;
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await UniTask.Yield(token);

            _testHelper.SetMaxTransitionCount(StateMachineZeroHistory.MaxTransition);

            _logger.LogStep("StateInitZeroHistory", $"Execute");

            return Transition.GoTo<StateFooHistory>();
        }
    }
}
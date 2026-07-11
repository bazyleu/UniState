using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.SubStateTests.Infrastructure
{
    internal class DrainCompositeState : DefaultCompositeState
    {
    }

    internal class DrainWinnerSubState : SubStateBase<DrainCompositeState>
    {
        private readonly ExecutionLogger _logger;

        public DrainWinnerSubState(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Disposables.Add(() => { _logger.LogStep("DrainWinnerSubState", "Disposables"); });

            await UniTask.Yield(token);

            _logger.LogStep("DrainWinnerSubState", "Execute");

            return Transition.GoToExit();
        }
    }

    internal class DrainSlowLoserSubState : SubStateBase<DrainCompositeState>
    {
        private readonly ExecutionLogger _logger;

        public DrainSlowLoserSubState(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Disposables.Add(() => { _logger.LogStep("DrainSlowLoserSubState", "Disposables"); });

            _logger.LogStep("DrainSlowLoserSubState", "ExecuteStart");

            // Ignores the token on purpose: emulates a sub state stuck in non-cancellable work.
            await UniTask.Yield();
            await UniTask.Yield();
            await UniTask.Yield();

            _logger.LogStep("DrainSlowLoserSubState", "ExecuteEnd");

            return Transition.GoToExit();
        }
    }

    internal class StateMachineCompositeDrain : VerifiableStateMachine, IStateMachineCompositeDrain
    {
        public StateMachineCompositeDrain(ExecutionLogger logger) : base(logger)
        {
        }

        protected override string ExpectedLog =>
            "DrainSlowLoserSubState (ExecuteStart) -> DrainWinnerSubState (Execute) -> " +
            "DrainSlowLoserSubState (ExecuteEnd) -> DrainWinnerSubState (Disposables) -> " +
            "DrainSlowLoserSubState (Disposables)";
    }

    public interface IStateMachineCompositeDrain : IVerifiableStateMachine
    {
    }
}

using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.CancellationTests.Infrastructure
{
    internal class CancelSourceState : StateBase
    {
        private readonly ExecutionLogger _logger;
        private readonly CancellationTestContext _context;

        public CancelSourceState(ExecutionLogger logger, CancellationTestContext context)
        {
            _logger = logger;
            _context = context;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Disposables.Add(() => { _logger.LogStep("CancelSourceState", "Disposables"); });

            _logger.LogStep("CancelSourceState", "Execute");

            _context.Source.Cancel();

            await UniTask.Yield(token);

            return Transition.GoToExit();
        }

        public override UniTask Exit(CancellationToken token)
        {
            _logger.LogStep("CancelSourceState", "Exit");

            return UniTask.CompletedTask;
        }
    }

    internal class StateMachineCancellation : VerifiableStateMachine, IStateMachineCancellation
    {
        private readonly ExecutionLogger _logger;

        public StateMachineCancellation(ExecutionLogger logger) : base(logger)
        {
            _logger = logger;
        }

        protected override void HandleStateChanged(StateMachineStateChangedData changeData)
        {
            if (changeData.ChangeType == StateMachineStateChangeType.Canceled)
            {
                _logger.LogStep(
                    nameof(StateMachineCancellation),
                    $"Canceled ({changeData.PreviousStateType?.Name ?? "None"})");
            }
        }

        protected override string ExpectedLog =>
            "CancelSourceState (Execute) -> StateMachineCancellation (Canceled (CancelSourceState)) -> " +
            "CancelSourceState (Disposables)";
    }

    public interface IStateMachineCancellation : IVerifiableStateMachine
    {
    }
}

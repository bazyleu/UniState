using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.CancellationTests.Infrastructure
{
    internal class ForeignCancellationState : StateBase
    {
        private readonly ExecutionLogger _logger;

        public ForeignCancellationState(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("ForeignCancellationState", "Execute");

            // Cancellation of an internal token that leaks out of the state as OperationCanceledException.
            // The state machine token is still alive, so this must be treated as a regular error.
            using var internalCts = new CancellationTokenSource();
            internalCts.Cancel();

            await UniTask.Yield(internalCts.Token);

            return Transition.GoToExit();
        }
    }

    internal class StateAfterRecovery : StateBase
    {
        private readonly ExecutionLogger _logger;

        public StateAfterRecovery(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("StateAfterRecovery", "Execute");

            return UniTask.FromResult(Transition.GoToExit());
        }
    }

    internal class StateMachineForeignCancellation : VerifiableStateMachine, IStateMachineForeignCancellation
    {
        private readonly ExecutionLogger _logger;

        public StateMachineForeignCancellation(ExecutionLogger logger) : base(logger)
        {
            _logger = logger;
        }

        protected override void HandleError(StateMachineErrorData errorData)
        {
            _logger.LogStep(
                nameof(StateMachineForeignCancellation),
                $"HandleError ({errorData.ErrorType}, {errorData.Exception.GetType().Name})");
        }

        protected override StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory)
            => transitionFactory.CreateStateTransition<StateAfterRecovery>();

        protected override string ExpectedLog =>
            "ForeignCancellationState (Execute) -> " +
            "StateMachineForeignCancellation (HandleError (StateExecuting, OperationCanceledException)) -> " +
            "StateAfterRecovery (Execute)";
    }

    public interface IStateMachineForeignCancellation : IVerifiableStateMachine
    {
    }
}

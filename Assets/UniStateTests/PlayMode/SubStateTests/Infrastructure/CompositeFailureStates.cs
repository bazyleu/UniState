using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.SubStateTests.Infrastructure
{
    internal class LoserFailureCompositeState : DefaultCompositeState
    {
    }

    internal class FailureWinnerSubState : SubStateBase<LoserFailureCompositeState>
    {
        private readonly ExecutionLogger _logger;

        public FailureWinnerSubState(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("FailureWinnerSubState", "Execute");

            return Transition.GoToExit();
        }
    }

    internal class ThrowingLoserSubState : SubStateBase<LoserFailureCompositeState>
    {
        private readonly ExecutionLogger _logger;

        public ThrowingLoserSubState(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("ThrowingLoserSubState", "ExecuteStart");

            // Ignores the token on purpose: the failure happens after the winner has already completed.
            await UniTask.Yield();
            await UniTask.Yield();
            await UniTask.Yield();

            _logger.LogStep("ThrowingLoserSubState", "Throw");

            throw new InvalidOperationException("Loser failure");
        }
    }

    internal class StateMachineCompositeLoserFailure : VerifiableStateMachine, IStateMachineCompositeLoserFailure
    {
        private readonly ExecutionLogger _logger;

        public StateMachineCompositeLoserFailure(ExecutionLogger logger) : base(logger)
        {
            _logger = logger;
        }

        protected override void HandleError(StateMachineErrorData errorData)
        {
            _logger.LogStep(
                nameof(StateMachineCompositeLoserFailure),
                $"HandleError ({errorData.ErrorType}, {errorData.Exception.GetType().Name})");
        }

        protected override StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory)
            => transitionFactory.CreateExitTransition();

        protected override string ExpectedLog =>
            "ThrowingLoserSubState (ExecuteStart) -> FailureWinnerSubState (Execute) -> " +
            "ThrowingLoserSubState (Throw) -> " +
            "StateMachineCompositeLoserFailure (HandleError (StateExecuting, InvalidOperationException))";
    }

    public interface IStateMachineCompositeLoserFailure : IVerifiableStateMachine
    {
    }

    internal class InitFailureCompositeState : DefaultCompositeState
    {
    }

    internal class FailingInitSubState : SubStateBase<InitFailureCompositeState>
    {
        private readonly ExecutionLogger _logger;

        public FailingInitSubState(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override UniTask Initialize(CancellationToken token)
        {
            _logger.LogStep("FailingInitSubState", "Initialize");

            throw new InvalidOperationException("Initialize failure");
        }

        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _logger.LogStep("FailingInitSubState", "Execute");

            return UniTask.FromResult(Transition.GoToExit());
        }
    }

    internal class SlowInitSubState : SubStateBase<InitFailureCompositeState>
    {
        private readonly ExecutionLogger _logger;

        public SlowInitSubState(ExecutionLogger logger)
        {
            _logger = logger;
        }

        public override async UniTask Initialize(CancellationToken token)
        {
            _logger.LogStep("SlowInitSubState", "InitializeStart");

            await UniTask.Yield();
            await UniTask.Yield();

            _logger.LogStep("SlowInitSubState", "InitializeEnd");
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await UniTask.Yield(token);

            _logger.LogStep("SlowInitSubState", "Execute");

            return Transition.GoToExit();
        }
    }

    internal class StateMachineCompositeInitFailure : VerifiableStateMachine, IStateMachineCompositeInitFailure
    {
        private readonly ExecutionLogger _logger;

        public StateMachineCompositeInitFailure(ExecutionLogger logger) : base(logger)
        {
            _logger = logger;
        }

        protected override void HandleError(StateMachineErrorData errorData)
        {
            _logger.LogStep(
                nameof(StateMachineCompositeInitFailure),
                $"HandleError ({errorData.ErrorType}, {errorData.Exception.GetType().Name})");
        }

        protected override StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory)
            => transitionFactory.CreateExitTransition();

        protected override string ExpectedLog =>
            "FailingInitSubState (Initialize) -> SlowInitSubState (InitializeStart, InitializeEnd) -> " +
            "StateMachineCompositeInitFailure (HandleError (StateInitializing, InvalidOperationException)) -> " +
            "FailingInitSubState (Execute)";
    }

    public interface IStateMachineCompositeInitFailure : IVerifiableStateMachine
    {
    }
}

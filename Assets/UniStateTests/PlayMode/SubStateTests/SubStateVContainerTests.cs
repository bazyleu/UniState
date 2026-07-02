using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.SubStateTests.Infrastructure;
using UnityEngine.TestTools;
using VContainer;

namespace UniStateTests.PlayMode.SubStateTests
{
    [TestFixture]
    public class SubStateVContainerTests : VContainerTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfStateSubStates_ExeptionRisedInSubState_AllSubStateDisposed() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IVerifiableStateMachine, StateInitial>(); });

        [UnityTest]
        public IEnumerator RunCompositeState_SubStateDisposeThrows_HandlesStateDisposingError() =>
            UniTask.ToCoroutine(async () =>
            {
                await RunAndVerify<IStateMachineSingleSubStateDisposeFailure, SingleDisposeFailureCompositeState>();
            });

        [UnityTest]
        public IEnumerator RunCompositeState_MultipleSubStateDisposeThrows_HandlesAggregateStateDisposingError() =>
            UniTask.ToCoroutine(async () =>
            {
                await RunAndVerify<IStateMachineMultipleSubStateDisposeFailure, MultipleDisposeFailureCompositeState>();
            });

        [UnityTest]
        public IEnumerator RunCompositeState_SlowSubStateIgnoresCancellation_ExecuteFinishesBeforeDispose() =>
            UniTask.ToCoroutine(async () =>
            {
                await RunAndVerify<IStateMachineCompositeDrain, DrainCompositeState>();
            });

        [UnityTest]
        public IEnumerator RunCompositeState_LosingSubStateThrowsAfterWinner_HandlesStateExecutingError() =>
            UniTask.ToCoroutine(async () =>
            {
                await RunAndVerify<IStateMachineCompositeLoserFailure, LoserFailureCompositeState>();
            });

        [UnityTest]
        public IEnumerator RunCompositeState_SubStateInitializeThrows_WaitsAllInitializeBeforeError() =>
            UniTask.ToCoroutine(async () =>
            {
                await RunAndVerify<IStateMachineCompositeInitFailure, InitFailureCompositeState>();
            });

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterStateMachine<IVerifiableStateMachine, StateMachineSubStates>();
            builder.RegisterStateMachine<IStateMachineSingleSubStateDisposeFailure, StateMachineSingleSubStateDisposeFailure>();
            builder.RegisterStateMachine<IStateMachineMultipleSubStateDisposeFailure, StateMachineMultipleSubStateDisposeFailure>();
            builder.RegisterStateMachine<IStateMachineCompositeDrain, StateMachineCompositeDrain>();
            builder.RegisterStateMachine<IStateMachineCompositeLoserFailure, StateMachineCompositeLoserFailure>();
            builder.RegisterStateMachine<IStateMachineCompositeInitFailure, StateMachineCompositeInitFailure>();

            builder.RegisterState<StateInitial>();
            builder.RegisterState<StateFinal>();
            builder.RegisterState<SubStateInitialFirst>();
            builder.RegisterState<SubStateInitialSecond>();
            builder.RegisterState<SubStateFinalFirst>();
            builder.RegisterState<SubStateFinalSecond>();

            builder.RegisterState<SingleDisposeFailureCompositeState>();
            builder.RegisterState<SingleThrowingDisposeSubState>();
            builder.RegisterState<SingleSuccessfulDisposeSubState>();
            builder.RegisterState<MultipleDisposeFailureCompositeState>();
            builder.RegisterState<MultipleFirstThrowingDisposeSubState>();
            builder.RegisterState<MultipleSecondThrowingDisposeSubState>();
            builder.RegisterState<MultipleSuccessfulDisposeSubState>();

            builder.RegisterState<DrainCompositeState>();
            builder.RegisterState<DrainWinnerSubState>();
            builder.RegisterState<DrainSlowLoserSubState>();
            builder.RegisterState<LoserFailureCompositeState>();
            builder.RegisterState<FailureWinnerSubState>();
            builder.RegisterState<ThrowingLoserSubState>();
            builder.RegisterState<InitFailureCompositeState>();
            builder.RegisterState<FailingInitSubState>();
            builder.RegisterState<SlowInitSubState>();
        }
    }
}

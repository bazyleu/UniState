using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Reflex.Core;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.SubStateTests.Infrastructure;
using UnityEngine.TestTools;

namespace UniStateTests.PlayMode.SubStateTests
{
    [TestFixture]
    public class SubStateReflexTests : ReflexTestsBase
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

        protected override void SetupBindings(ContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterStateMachine(typeof(StateMachineSubStates), typeof(IVerifiableStateMachine));
            builder.RegisterStateMachine(
                typeof(StateMachineSingleSubStateDisposeFailure),
                typeof(IStateMachineSingleSubStateDisposeFailure));
            builder.RegisterStateMachine(
                typeof(StateMachineMultipleSubStateDisposeFailure),
                typeof(IStateMachineMultipleSubStateDisposeFailure));
            builder.RegisterStateMachine(
                typeof(StateMachineCompositeDrain),
                typeof(IStateMachineCompositeDrain));
            builder.RegisterStateMachine(
                typeof(StateMachineCompositeLoserFailure),
                typeof(IStateMachineCompositeLoserFailure));
            builder.RegisterStateMachine(
                typeof(StateMachineCompositeInitFailure),
                typeof(IStateMachineCompositeInitFailure));

            builder.RegisterState(typeof(StateInitial));
            builder.RegisterState(typeof(StateFinal));
            builder.RegisterState(typeof(SubStateInitialFirst));
            builder.RegisterState(typeof(SubStateInitialSecond));
            builder.RegisterState(typeof(SubStateFinalFirst));
            builder.RegisterState(typeof(SubStateFinalSecond));

            builder.RegisterState(typeof(SingleDisposeFailureCompositeState));
            builder.RegisterState(typeof(SingleThrowingDisposeSubState));
            builder.RegisterState(typeof(SingleSuccessfulDisposeSubState));
            builder.RegisterState(typeof(MultipleDisposeFailureCompositeState));
            builder.RegisterState(typeof(MultipleFirstThrowingDisposeSubState));
            builder.RegisterState(typeof(MultipleSecondThrowingDisposeSubState));
            builder.RegisterState(typeof(MultipleSuccessfulDisposeSubState));

            builder.RegisterState(typeof(DrainCompositeState));
            builder.RegisterState(typeof(DrainWinnerSubState));
            builder.RegisterState(typeof(DrainSlowLoserSubState));
            builder.RegisterState(typeof(LoserFailureCompositeState));
            builder.RegisterState(typeof(FailureWinnerSubState));
            builder.RegisterState(typeof(ThrowingLoserSubState));
            builder.RegisterState(typeof(InitFailureCompositeState));
            builder.RegisterState(typeof(FailingInitSubState));
            builder.RegisterState(typeof(SlowInitSubState));
        }
    }
}

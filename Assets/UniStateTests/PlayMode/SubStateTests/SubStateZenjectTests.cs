using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.SubStateTests.Infrastructure;
using UnityEngine.TestTools;
using Zenject;

namespace UniStateTests.PlayMode.SubStateTests
{
    [TestFixture]
    public class SubStateZenjectTests : ZenjectTestsBase
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

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.BindStateMachine<IVerifiableStateMachine, StateMachineSubStates>();
            container.BindStateMachine<IStateMachineSingleSubStateDisposeFailure, StateMachineSingleSubStateDisposeFailure>();
            container.BindStateMachine<IStateMachineMultipleSubStateDisposeFailure, StateMachineMultipleSubStateDisposeFailure>();
            container.BindStateMachine<IStateMachineCompositeDrain, StateMachineCompositeDrain>();
            container.BindStateMachine<IStateMachineCompositeLoserFailure, StateMachineCompositeLoserFailure>();
            container.BindStateMachine<IStateMachineCompositeInitFailure, StateMachineCompositeInitFailure>();

            container.BindState<StateInitial>();
            container.BindState<StateFinal>();
            container.BindState<SubStateInitialFirst>();
            container.BindState<SubStateInitialSecond>();
            container.BindState<SubStateFinalFirst>();
            container.BindState<SubStateFinalSecond>();

            container.BindState<SingleDisposeFailureCompositeState>();
            container.BindState<SingleThrowingDisposeSubState>();
            container.BindState<SingleSuccessfulDisposeSubState>();
            container.BindState<MultipleDisposeFailureCompositeState>();
            container.BindState<MultipleFirstThrowingDisposeSubState>();
            container.BindState<MultipleSecondThrowingDisposeSubState>();
            container.BindState<MultipleSuccessfulDisposeSubState>();

            container.BindState<DrainCompositeState>();
            container.BindState<DrainWinnerSubState>();
            container.BindState<DrainSlowLoserSubState>();
            container.BindState<LoserFailureCompositeState>();
            container.BindState<FailureWinnerSubState>();
            container.BindState<ThrowingLoserSubState>();
            container.BindState<InitFailureCompositeState>();
            container.BindState<FailingInitSubState>();
            container.BindState<SlowInitSubState>();
        }
    }
}

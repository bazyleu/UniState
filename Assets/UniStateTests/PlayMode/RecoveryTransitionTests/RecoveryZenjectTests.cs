using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.HistoryTests.Infrastructure;
using UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure;
using UnityEngine.TestTools;
using Zenject;

namespace UniStateTests.PlayMode.RecoveryTransitionTests
{
    [TestFixture]
    public class RecoveryZenjectTests : ZenjectTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfStateWithDefaultRecovery_ExceptionDuringExecute_StateMachineExecuteGoBack() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<StateMachineDefaultRecovery, StateInitial>(); });

        [UnityTest]
        public IEnumerator RunChaneOfStateWithGoToStateRecovery_ExceptionDuringExecute_StateMachineGoToRecoveryState() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<StateMachineGoToStateRecovery, StateInitial>(); });

        [UnityTest]
        public IEnumerator RunChaneOfStateWithExitRecovery_ExceptionDuringExecute_StateMachineExit() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<StateMachineExitRecovery, StateInitial>(); });

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.BindInterfacesAndSelfTo<RecoveryTestHelper>().AsSingle();

            container.BindStateMachine<StateMachineDefaultRecovery>();
            container.BindStateMachine<StateMachineGoToStateRecovery>();
            container.BindStateMachine<StateMachineExitRecovery>();

            container.BindState<StateInitial>();
            container.BindState<StateThrowTwoException>();
            container.BindState<StateWithFailExecution>();
            container.BindState<StateStartedAfterException>();
        }
    }
}
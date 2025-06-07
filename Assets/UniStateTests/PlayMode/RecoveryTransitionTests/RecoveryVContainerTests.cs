using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.RecoveryTransitionTests.Infrastructure;
using UnityEngine.TestTools;
using VContainer;

namespace UniStateTests.PlayMode.RecoveryTransitionTests
{
    [TestFixture]
    public class RecoveryVContainerTests : VContainerTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfStateWithDefaultRecovery_ExceptionDuringExecute_StateMachineExecuteGoBack() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IStateMachineDefaultRecovery, StateInitial>(); });

        [UnityTest]
        public IEnumerator RunChaneOfStateWithGoToStateRecovery_ExceptionDuringExecute_StateMachineGoToRecoveryState() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IStateMachineGoToStateRecovery, StateInitial>(); });

        [UnityTest]
        public IEnumerator RunChaneOfStateWithExitRecovery_ExceptionDuringExecute_StateMachineExit() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IStateMachineExitRecovery, StateInitial>(); });

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.Register<RecoveryTestHelper>(Lifetime.Singleton);

            builder.RegisterStateMachine<IStateMachineDefaultRecovery, StateMachineDefaultRecovery>();
            builder.RegisterStateMachine<IStateMachineGoToStateRecovery, StateMachineGoToStateRecovery>();
            builder.RegisterStateMachine<IStateMachineExitRecovery, StateMachineExitRecovery>();

            builder.RegisterState<StateInitial>();
            builder.RegisterState<StateThrowTwoException>();
            builder.RegisterState<StateWithFailExecution>();
            builder.RegisterState<StateStartedAfterException>();
        }
    }
}
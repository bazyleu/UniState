using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.HistoryTests.Infrastructure;
using UniStateTests.PlayMode.SubStateTests.Infrastructure;
using UnityEngine.TestTools;
using VContainer;
using Zenject;

namespace UniStateTests.PlayMode.SubStateTests
{
    [TestFixture]
    public class SubStateVContainerTests : VContainerTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfStateSubStates_ExeptionRisedInSubState_AllSubStateDisposed() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IVerifiableStateMachine, StateInitial>(); });

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterStateMachine<IVerifiableStateMachine, StateMachineSubStates>();

            builder.RegisterState<StateInitial>();
            builder.RegisterState<StateFinal>();
            builder.RegisterState<SubStateInitialFirst>();
            builder.RegisterState<SubStateInitialSecond>();
            builder.RegisterState<SubStateFinalFirst>();
            builder.RegisterState<SubStateFinalSecond>();
        }
    }
}
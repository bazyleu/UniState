using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.HistoryTests.Infrastructure;
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

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.BindStateMachine<IVerifiableStateMachine, StateMachineSubStates>();

            container.BindState<StateInitial>();
            container.BindState<StateFinal>();
            container.BindState<SubStateInitialFirst>();
            container.BindState<SubStateInitialSecond>();
            container.BindState<SubStateFinalFirst>();
            container.BindState<SubStateFinalSecond>();
        }
    }
}
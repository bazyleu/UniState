using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.GoToStateTests.Infrastructure;
using UnityEngine.TestTools;
using Zenject;

namespace UniStateTests.PlayMode.GoToStateTests
{
    [TestFixture]
    internal class ZenjectIntegrationTests : ZenjectTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfState_ExitFromChain_ChainExecutedCorrectly() => UniTask.ToCoroutine(async () =>
        {
            await RunAndVerify<IVerifiableStateMachine, StateGoTo1>();
        });

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.BindStateMachine<IVerifiableStateMachine, StateMachineGoToState>();
            container.BindState<StateGoTo1>();
            container.BindState<StateGoTo2>();
            container.BindState<StateGoToAbstract3, StateGoTo3>();
            container.BindState<StateGoTo4>();
            container.BindState<StateGoTo5>();
            container.BindState<CompositeStateGoTo6>();
            container.BindState<SubStateGoTo6First>();
            container.BindState<SubStateGoTo6Second>();
            container.BindState<CompositeStateGoTo7>();
            container.BindState<SubStateGoTo7First>();
            container.BindState<SubStateGoTo7Second>();
            container.BindState<StateGoTo8>();
        }
    }
}
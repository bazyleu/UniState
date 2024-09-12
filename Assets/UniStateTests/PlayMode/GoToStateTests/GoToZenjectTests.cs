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
            await RunAndVerify<StateMachineGoToState, StateGoTo1>();
        });

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.BindStateMachine<StateMachineGoToState>();
            container.BindState<StateGoTo1>();
            container.BindState<StateGoTo2>();
            container.BindAbstractState<StateGoToAbstract3, StateGoTo3>();
            container.BindState<StateGoTo4>();
            container.BindState<StateGoTo5>();
            container.BindState<CompositeStateGoTo6>();
            container.BindState<SubStateGoToX6A>();
            container.BindState<SubStateGoToX6B>();
            container.BindState<CompositeStateGoTo7>();
            container.BindState<SubStateGoToX7A>();
            container.BindState<SubStateGoToX7B>();
            container.BindState<StateGoTo8>();
        }
    }
}
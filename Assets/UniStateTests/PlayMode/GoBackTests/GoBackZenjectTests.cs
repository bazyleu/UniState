using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.GoBackTests.Infrastructure;
using UnityEngine.TestTools;
using Zenject;

namespace UniStateTests.PlayMode.GoBackTests
{
    [TestFixture]
    internal class GoBackZenjectTests : ZenjectTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfState_GoBackFromTheChain_ExitFromStateMachineWithCorrectOrderOfStates() =>
            UniTask.ToCoroutine(async () =>
            {
                await RunAndVerify<StateMachineGoBack, StateGoBack1>();
            });

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.Bind<GoBackFlagsData>().ToSelf().AsSingle();

            container.BindStateMachine<StateMachineGoBack>();
            container.BindState<StateGoBack1>();
            container.BindState<StateGoBack2>();
            container.BindState<StateGoBack3>();
        }
    }
}
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
                await RunAndVerify<IVerifiableStateMachine, StateGoBackFirst>();
            });

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.Bind<GoBackTestHelper>().ToSelf().AsSingle();

            container.BindStateMachine<IVerifiableStateMachine, StateMachineGoBack>();
            container.BindState<StateGoBackFirst>();
            container.BindState<StateGoBackSecond>();
            container.BindState<StateGoBackThird>();
        }
    }
}
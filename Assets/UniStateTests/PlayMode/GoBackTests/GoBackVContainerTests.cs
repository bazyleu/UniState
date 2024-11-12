using System.Collections;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.GoBackTests.Infrastructure;
using UnityEngine.TestTools;
using VContainer;

namespace UniStateTests.PlayMode.GoBackTests
{
    public class GoBackVContainerTests : VContainerTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfState_GoBackFromTheChain_ExitFromStateMachineWithCorrectOrderOfStates() =>
            UniTask.ToCoroutine(async () =>
            {
                await RunAndVerify<StateMachineGoBack, StateGoBackFirst>();
            });

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.Register<GoBackTestHelper>(Lifetime.Singleton);

            builder.RegisterStateMachine<StateMachineGoBack>();
            builder.RegisterState<StateGoBackFirst>();
            builder.RegisterState<StateGoBackSecond>();
            builder.RegisterState<StateGoBackThird>();
        }
    }
}
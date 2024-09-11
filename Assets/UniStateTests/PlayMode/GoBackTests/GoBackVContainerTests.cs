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
                await RunAndVerify<StateMachineGoBack, StateGoBack1>();
            });

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.Register<GoBackFlagsData>(Lifetime.Singleton);

            builder.RegisterStateMachine<StateMachineGoBack>();
            builder.RegisterState<StateGoBack1>();
            builder.RegisterState<StateGoBack2>();
            builder.RegisterState<StateGoBack3>();
        }
    }
}
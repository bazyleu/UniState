using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.GoToStateTests.Infrastructure;
using UnityEngine.TestTools;
using VContainer;

namespace UniStateTests.PlayMode.GoToStateTests
{
    [TestFixture]
    internal class GoToVContainerTests : VContainerTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfState_ExitFromChain_ChainExecutedCorrectly() => UniTask.ToCoroutine(async () =>
        {
            await RunAndVerify<IVerifiableStateMachine, StateGoTo1>();
        });

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterStateMachine<IVerifiableStateMachine, StateMachineGoToState>();
            builder.RegisterState<StateGoTo1>();
            builder.RegisterState<StateGoTo2>();
            builder.RegisterState<StateGoTo3>();
            builder.RegisterState<StateGoToAbstract3, StateGoTo3>();
            builder.RegisterState<StateGoTo4>();
            builder.RegisterState<StateGoTo5>();
            builder.RegisterState<CompositeStateGoTo6>();
            builder.RegisterState<SubStateGoTo6First>();
            builder.RegisterState<SubStateGoTo6Second>();
            builder.RegisterState<CompositeStateGoTo7>();
            builder.RegisterState<SubStateGoTo7First>();
            builder.RegisterState<SubStateGoTo7Second>();
            builder.RegisterState<StateGoTo8>();
        }
    }
}
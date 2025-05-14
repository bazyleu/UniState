using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.StateBehaviorAttributeTests.Infrastructure;
using UnityEngine.TestTools;
using VContainer;

namespace UniStateTests.PlayMode.StateBehaviorAttributeTests
{
    [TestFixture]
    public class BehaviorAttributeVContainerTests : VContainerTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfStateWithAttributes_ExitFromChain_ChainExecutedCorrectly() => UniTask.ToCoroutine(async () =>
        {
            await RunAndVerify<StateMachineBehaviourAttribute, FirstState>();
        });

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterStateMachine<StateMachineBehaviourAttribute>();
            builder.RegisterState<FirstState>();
            builder.RegisterState<NoReturnState>();
            builder.RegisterState<FastInitializeState>();

            builder.Register<BehaviourAttributeTestHelper>(Lifetime.Singleton);
        }
    }
}
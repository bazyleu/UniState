using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.Execution.Infrastructure;
using UnityEngine.TestTools;
using Zenject;
using FirstState = UniStateTests.PlayMode.Execution.Infrastructure.FirstState;

namespace UniStateTests.PlayMode.Execution
{
    [TestFixture]
    public class ExecutionZenjectTests : ZenjectTestsBase
    {
        [UnityTest]
        public IEnumerator RunStateMachineSeveralTime_EndExecution_ExecutionStatusValid() => UniTask.ToCoroutine(
            async () =>
            {
                var testHelper = Container.Resolve<ExecutionTestHelper>();
                testHelper.SetPath(StateMachineExecutionType.Default);

                await RunAndVerify<ExecutionStateMachine, FirstState>();
                Assert.False(testHelper.CurrentStateMachine.IsExecuting);

                testHelper.SetPath(StateMachineExecutionType.WrongDependency);

                await RunAndVerify<ExecutionStateMachine, FirstState>();
                Assert.False(testHelper.CurrentStateMachine.IsExecuting);

                testHelper.SetPath(StateMachineExecutionType.Exception);

                await RunAndVerify<ExecutionStateMachine, FirstState>();
                Assert.False(testHelper.CurrentStateMachine.IsExecuting);
            });

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.BindInterfacesAndSelfTo<ExecutionTestHelper>().AsSingle();

            container.BindStateMachine<ExecutionStateMachine>();

            container.BindState<FirstState>();
            container.BindState<SecondState>();
            container.BindState<SecondStateWithException>();
            container.BindState<SecondStateWithWrongDependency>();
 }
    }
}
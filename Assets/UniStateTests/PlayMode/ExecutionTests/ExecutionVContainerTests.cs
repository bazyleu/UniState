using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.Execution.Infrastructure;
using UnityEngine.TestTools;
using VContainer;
using FirstState = UniStateTests.PlayMode.Execution.Infrastructure.FirstState;

namespace UniStateTests.PlayMode.Execution
{
    [TestFixture]
    public class ExecutionVContainerTests : VContainerTestsBase
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


        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.Register<ExecutionTestHelper>(Lifetime.Singleton);

            builder.RegisterStateMachine<ExecutionStateMachine>();

            builder.RegisterState<FirstState>();
            builder.RegisterState<SecondState>();
            builder.RegisterState<SecondStateWithException>();
            builder.RegisterState<SecondStateWithWrongDependency>();
        }
    }
}
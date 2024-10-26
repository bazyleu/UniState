using System.Collections;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.HistoryTests.Infrastructure;
using UnityEngine.TestTools;
using VContainer;

namespace UniStateTests.PlayMode.HistoryTests
{
    public class HistorySizeVContainerTests : VContainerTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfStateWithLongHistory_GoBack_ChainExecutedCorrectly() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<StateMachineLongHistory, StateInitLongHistory>(); });

        [UnityTest]
        public IEnumerator RunChaneOfStateWithZeroHistory_GoBack_ExitFromStateMachine() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<StateMachineZeroHistory, StateInitZeroHistory>(); });

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.Register<HistorySizeTestHelper>(Lifetime.Singleton);

            builder.RegisterStateMachine<StateMachineLongHistory>();
            builder.RegisterStateMachine<StateMachineZeroHistory>();
            builder.RegisterState<StateInitLongHistory>();
            builder.RegisterState<StateInitZeroHistory>();
            builder.RegisterState<StateFooHistory>();
            builder.RegisterState<StateBarHistory>();
        }
    }
}
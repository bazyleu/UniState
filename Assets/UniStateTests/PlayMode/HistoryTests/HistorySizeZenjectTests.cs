using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.HistoryTests.Infrastructure;
using UnityEngine.TestTools;
using Zenject;

namespace UniStateTests.PlayMode.HistoryTests
{
    [TestFixture]
    public class HistorySizeZenjectTests : ZenjectTestsBase
    {
        [UnityTest]
        public IEnumerator RunChaneOfStateWithLongHistory_GoBack_ChainExecutedCorrectly() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IStateMachineLongHistory, StateInitLongHistory>(); });

        [UnityTest]
        public IEnumerator RunChaneOfStateWithZeroHistory_GoBack_ExitFromStateMachine() =>
            UniTask.ToCoroutine(async () => { await RunAndVerify<IStateMachineZeroHistory, StateInitZeroHistory>(); });

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.BindInterfacesAndSelfTo<HistorySizeTestHelper>().AsSingle();

            container.BindStateMachine<IStateMachineLongHistory, StateMachineLongHistory>();
            container.BindStateMachine<IStateMachineZeroHistory, StateMachineZeroHistory>();
            container.BindState<StateInitLongHistory>();
            container.BindState<StateInitZeroHistory>();
            container.BindState<StateFooHistory>();
            container.BindState<StateBarHistory>();
        }
    }
}
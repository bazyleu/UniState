using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.Common;
using UniStateTests.PlayMode.GoBack;
using UnityEngine.TestTools;
using Zenject;

namespace UniStateTests.PlayMode
{
    [TestFixture]
    public class GoBackTests : ZenjectTestsBase
    {
        protected override void SetupBindings()
        {
            Container.BindInterfacesAndSelfTo<ExecutionLogger>().AsSingle();
            Container.Bind<GoBackTestFlags>().ToSelf().AsSingle();

            Container.Bind<StateMachine>().ToSelf().AsTransient();
            Container.Bind<StateGoBack1>().ToSelf().AsTransient();
            Container.Bind<StateGoBack2>().ToSelf().AsTransient();
            Container.Bind<StateGoBack3>().ToSelf().AsTransient();
        }

        [UnityTest]
        public IEnumerator RunChaneOfState_GoBackFromTheChain_ExitFromStateMachineWithCorrectOrderOfStates() =>
            UniTask.ToCoroutine(async () =>
            {
                const string expectedLog =
                    "StateMachine (Started) -> StateGoBack1 (Execute) -> StateGoBack2 (Execute) -> StateGoBack3 (Execute) -> " +
                    "StateGoBack2 (Execute) -> StateGoBack1 (Execute) -> StateMachine (Finished)";

                var logger = Container.Resolve<ExecutionLogger>();

                logger.LogStep("StateMachine", "Started");

                var stateMachine = StateMachineHelper.CreateStateMachine<StateMachine>(Container.ToTypeResolver());
                await stateMachine.Execute<StateGoBack1>(GetTimeoutToken());

                logger.LogStep("StateMachine", "Finished");

                var actualLog = logger.FinishLogging();

                Assert.AreEqual(expectedLog, actualLog);
            });
    }
}
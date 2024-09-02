using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.PlayMode.Common;
using UniStateTests.PlayMode.GoBack;
using UnityEngine.TestTools;
using Zenject;

namespace UniStateTests.PlayMode
{
    [TestFixture]
    public class GoBackTests
    {
        [UnityTest]
        public IEnumerator RunChaneOfState_GoBackFromTheChain_ExitFromStateMachineWithCorrectOrderOfStates() => UniTask.ToCoroutine(async () =>
        {
            //TODO: Refactor this test

            const string expectedLog = @"Create and execute State Machine
StateGoBack1 - Execute
StateGoBack2 - Execute
StateGoBack3 - Execute
StateGoBack2 - Execute
StateGoBack1 - Execute
State Machine is finished
";

            var container = new DiContainer(StaticContext.Container);

            container.BindInterfacesTo<StateLogger>().AsSingle();
            container.Bind<GoBackTestFlags>().ToSelf().AsSingle();

            container.Bind<StateMachine>().ToSelf().AsTransient();
            container.Bind<StateGoBack1>().ToSelf().AsTransient();
            container.Bind<StateGoBack2>().ToSelf().AsTransient();
            container.Bind<StateGoBack3>().ToSelf().AsTransient();

            CancellationTokenSource cts = new CancellationTokenSource();

            //TODO: Move to base class
            using var slim = cts.CancelAfterSlim(TimeSpan.FromSeconds(5), DelayType.UnscaledDeltaTime);

            var logger = container.Resolve<IStateLogger>();

            logger.LogLine("Create and execute State Machine");
            var stateMachine =  StateMachineHelper.CreateStateMachine<StateMachine>(container.ToTypeResolver());
            await stateMachine.Execute<StateGoBack1>(cts.Token);

            logger.LogLine("State Machine is finished");

            var actualLog = logger.GetFullLog();

            //TODO: Move to base class
            StaticContext.Clear();

            Assert.AreEqual(actualLog, expectedLog);
        });
    }
}
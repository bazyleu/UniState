using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.PlayMode.States;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace UniStateTests.PlayMode
{
    [TestFixture]
    public class ZenjectIntegrationTest
    {
        [UnityTest]
        public IEnumerator SimpleRun() => UniTask.ToCoroutine(async () =>
        {
            var container = new DiContainer(StaticContext.Container);

            container.Bind<SimpleStateMachine>().ToSelf().AsTransient();
            container.Bind<Test1State>().ToSelf().AsTransient();
            container.Bind<Test2State>().ToSelf().AsTransient();
            container.Bind<Test3StateAbstract>().To<Test3State>().AsTransient();
            container.Bind<ITest4State>().To<Test4State>().AsTransient();

            CancellationTokenSource cts = new CancellationTokenSource();

            using var slim = cts.CancelAfterSlim(TimeSpan.FromSeconds(5), DelayType.UnscaledDeltaTime);

            var stateMachine =  StateMachineHelper.CreateStateMachine<SimpleStateMachine>(container.ToTypeResolver());

            await stateMachine.Execute<Test1State>(cts.Token);

            Assert.Pass();
        });
    }
}
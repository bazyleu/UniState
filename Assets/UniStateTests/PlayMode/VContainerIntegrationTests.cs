using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.PlayMode.States;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;
using Object = UnityEngine.Object;


namespace UniStateTests.PlayMode
{
    [TestFixture]
    public class VContainerIntegrationTests
    {
        private GameObject _containerHolder;
        private IObjectResolver _objectResolver;

        [SetUp]
        public void Setup()
        {
           _containerHolder = new GameObject("container");
            var component = _containerHolder.AddComponent<MainLifetimeScope>();
           _objectResolver = component.Container;
        }

        [TearDown]
        public void TearDown()
        {
           Object.Destroy(_containerHolder);
        }

        [UnityTest]
        public IEnumerator SimpleRun() => UniTask.ToCoroutine(async () =>
        {
             CancellationTokenSource cts = new CancellationTokenSource();

             using var slim = cts.CancelAfterSlim(TimeSpan.FromSeconds(5), DelayType.UnscaledDeltaTime);

             var stateMachine =  StateMachineHelper.CreateStateMachine<SimpleStateMachine>(_objectResolver.ToTypeResolver());

             await stateMachine.Execute<Test1State>(cts.Token);

            Assert.Pass();
        });
    }
}
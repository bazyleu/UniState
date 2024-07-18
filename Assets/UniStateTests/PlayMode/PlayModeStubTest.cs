using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;
using Object = UnityEngine.Object;


namespace UniStateTests.PlayMode.States
{
    [TestFixture]
    public class PlayModeStubTest
    {
        private GameObject _containerHolder;
        private IObjectResolver _objectResolver;

        [OneTimeSetUp]
        public void Setup()
        {
           _containerHolder = new GameObject("container");
            var component = _containerHolder.AddComponent<MainLifetimeScope>();
           _objectResolver = component.Container;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
           Object.Destroy(_containerHolder);
        }

        [UnityTest]
        public IEnumerator SuccessTest() => UniTask.ToCoroutine(async () =>
        {
            //TODO: new VContainerTypeResolver???

             CancellationTokenSource cts = new CancellationTokenSource();

             using var slim = cts.CancelAfterSlim(TimeSpan.FromSeconds(5), DelayType.UnscaledDeltaTime);

             var stateMachine = new SimpleStateMachine();
             stateMachine.Initialize(new VContainerTypeResolver(_objectResolver));

             await stateMachine.Execute<Test1State>(cts.Token);

            Assert.Pass();
        });



        [UnityTest]
        public IEnumerator FailTest() => UniTask.ToCoroutine(async () =>
        {
            Debug.Log("T1 " + Time.unscaledTime);
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            Debug.Log("T2 " + Time.unscaledTime);
            Assert.Fail();
        });
    }
}
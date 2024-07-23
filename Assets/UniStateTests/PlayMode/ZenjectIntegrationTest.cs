using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
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
        public IEnumerator ZenjectTest() => UniTask.ToCoroutine(async () =>
        {
            var container = new DiContainer(StaticContext.Container);

            container.Bind<SimpleStateMachine>().ToSelf().AsTransient();
            container.Bind<Test1State>().ToSelf().AsTransient();
            container.Bind<Test2State>().ToSelf().AsTransient();
            container.Bind<Test3StateAbstract>().To<Test3StateAbstract>().AsTransient();
            container.Bind<ITest4State>().To<Test4State>().AsTransient();




            Assert.Pass();


        });
    }
}
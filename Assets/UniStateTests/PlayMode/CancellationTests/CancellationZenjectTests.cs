using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.CancellationTests.Infrastructure;
using UnityEngine.TestTools;
using Zenject;

namespace UniStateTests.PlayMode.CancellationTests
{
    [TestFixture]
    public class CancellationZenjectTests : ZenjectTestsBase
    {
        [UnityTest]
        public IEnumerator RunStateMachine_ForeignOperationCanceled_HandlesErrorAndRecovers() =>
            UniTask.ToCoroutine(async () =>
            {
                await RunAndVerify<IStateMachineForeignCancellation, ForeignCancellationState>();
            });

        [UnityTest]
        public IEnumerator RunStateMachine_TokenCanceled_SkipsExitDisposesAndReportsCanceled() =>
            UniTask.ToCoroutine(async () =>
            {
                await CancellationTestHelper.RunAndVerifyCanceled<IStateMachineCancellation, CancelSourceState>(
                    Container.ToTypeResolver(),
                    Container.Resolve<CancellationTestContext>(),
                    GetTimeoutToken());
            });

        protected override void SetupBindings(DiContainer container)
        {
            base.SetupBindings(container);

            container.BindInterfacesAndSelfTo<CancellationTestContext>().AsSingle();

            container.BindStateMachine<IStateMachineForeignCancellation, StateMachineForeignCancellation>();
            container.BindStateMachine<IStateMachineCancellation, StateMachineCancellation>();

            container.BindState<ForeignCancellationState>();
            container.BindState<StateAfterRecovery>();
            container.BindState<CancelSourceState>();
        }
    }
}

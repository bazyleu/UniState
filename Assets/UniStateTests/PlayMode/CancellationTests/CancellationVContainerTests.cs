using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.CancellationTests.Infrastructure;
using UnityEngine.TestTools;
using VContainer;

namespace UniStateTests.PlayMode.CancellationTests
{
    [TestFixture]
    public class CancellationVContainerTests : VContainerTestsBase
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

        protected override void SetupBindings(IContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.Register<CancellationTestContext>(Lifetime.Singleton);

            builder.RegisterStateMachine<IStateMachineForeignCancellation, StateMachineForeignCancellation>();
            builder.RegisterStateMachine<IStateMachineCancellation, StateMachineCancellation>();

            builder.RegisterState<ForeignCancellationState>();
            builder.RegisterState<StateAfterRecovery>();
            builder.RegisterState<CancelSourceState>();
        }
    }
}

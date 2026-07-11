using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;
using UniState;
using UniStateTests.Common;
using UniStateTests.PlayMode.CancellationTests.Infrastructure;
using UnityEngine.TestTools;

namespace UniStateTests.PlayMode.CancellationTests
{
    [TestFixture]
    public class CancellationReflexTests : ReflexTestsBase
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

        protected override void SetupBindings(ContainerBuilder builder)
        {
            base.SetupBindings(builder);

            builder.RegisterType(typeof(CancellationTestContext), Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterStateMachine(
                typeof(StateMachineForeignCancellation),
                typeof(IStateMachineForeignCancellation));
            builder.RegisterStateMachine(
                typeof(StateMachineCancellation),
                typeof(IStateMachineCancellation));

            builder.RegisterState(typeof(ForeignCancellationState));
            builder.RegisterState(typeof(StateAfterRecovery));
            builder.RegisterState(typeof(CancelSourceState));
        }
    }
}

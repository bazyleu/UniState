using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;
using UniStateTests.Common;

namespace UniStateTests.PlayMode.CancellationTests.Infrastructure
{
    public static class CancellationTestHelper
    {
        public static async UniTask RunAndVerifyCanceled<TStateMachine, TState>(
            ITypeResolver typeResolver,
            CancellationTestContext context,
            CancellationToken timeoutToken)
            where TStateMachine : class, IStateMachine, IVerifiableStateMachine
            where TState : class, IState<EmptyPayload>
        {
            var stateMachine = typeResolver.Resolve<TStateMachine>();

            using var source = CancellationTokenSource.CreateLinkedTokenSource(timeoutToken);
            context.Source = source;

            var canceled = false;

            try
            {
                await stateMachine.Execute<TState>(source.Token);
            }
            catch (OperationCanceledException)
            {
                canceled = true;
            }

            Assert.IsTrue(canceled, "Execute() must propagate OperationCanceledException when its token is canceled.");
            Assert.IsFalse(stateMachine.IsExecuting);

            stateMachine.Verify();
        }
    }
}

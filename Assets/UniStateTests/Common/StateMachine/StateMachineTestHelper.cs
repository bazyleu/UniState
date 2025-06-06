using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

namespace UniStateTests.Common
{
    public static class StateMachineTestHelper
    {
        public static async UniTask RunAndVerify<TStateMachine, TState>(ITypeResolver typeResolver,
            CancellationToken cancellationToken)
            where TStateMachine : class, IStateMachine, IVerifiableStateMachine
            where TState : class, IState<EmptyPayload>
        {
            var stateMachine = typeResolver.Resolve<TStateMachine>();
            await stateMachine.Execute<TState>(cancellationToken);

            stateMachine.Verify();
        }
    }
}
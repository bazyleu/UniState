using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

namespace UniStateTests.PlayMode.StateMachineTests.Infrastructure
{
    public class SecondStateWithException : StateBase
    {
        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            return UniTask.FromResult(Transition.GoToExit());
        }

        public override UniTask Exit(CancellationToken token)
        {
            throw new Exception("test exception");
        }
    }
}
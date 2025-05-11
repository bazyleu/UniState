using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

namespace UniStateTests.PlayMode.StateMachineTests.Infrastructure
{
    public class SecondState: StateBase
    {
        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

namespace Benchmarks.UniStateFixtures
{
    public class BarState : StateBase
    {
        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            return UniTask.FromResult(Transition.GoTo<FooState>());
        }
    }
}
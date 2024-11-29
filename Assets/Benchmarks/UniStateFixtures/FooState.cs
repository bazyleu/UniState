using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

namespace Benchmarks.UniStateFixtures
{
    public class FooState : StateBase
    {
        //TODO: Add Ininit and Exit to routine classes too. Increase counter
        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            return UniTask.FromResult(Transition.GoTo<BarState>());
        }
    }
}
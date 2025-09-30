using System.Threading;
using Benchmarks.Common;
using Cysharp.Threading.Tasks;
using UniState;

namespace Benchmarks.UniStateFixtures
{
    public class FooState : StateBase
    {
        private readonly BenchmarkHelper _benchmarkHelper;

        public FooState(BenchmarkHelper benchmarkHelper)
        {
            _benchmarkHelper = benchmarkHelper;
        }

        public override UniTask InitializeAsync(CancellationToken token)
        {
            _benchmarkHelper.InitializeWasInvoked();

            return UniTask.CompletedTask;
        }

        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            _benchmarkHelper.ExecuteWasInvoked();

            return UniTask.FromResult(_benchmarkHelper.TargetReached()
                ? Transition.GoToExit()
                : Transition.GoTo<BarState>());
        }

        public override UniTask ExitAsync(CancellationToken token)
        {
            _benchmarkHelper.ExitWasInvoked();

            return UniTask.CompletedTask;
        }
    }
}
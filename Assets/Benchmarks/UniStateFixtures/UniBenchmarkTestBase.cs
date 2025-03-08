using System.Threading;
using Benchmarks.Common;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace Benchmarks.UniStateFixtures
{
    public class UniBenchmarkTestBase : MonoBehaviour
    {
        protected async UniTask Run(int stateCount, bool withoutHistory)
        {
            var benchmarkHelper = new BenchmarkHelper();
            var resolver = new SimpleResolver(benchmarkHelper);

            benchmarkHelper.SetStatesCountTarget(stateCount);

            IExecutableStateMachine stateMachine =
                withoutHistory
                    ? StateMachineHelper.CreateStateMachine<StateMachineWithoutHistory>(resolver)
                    : StateMachineHelper.CreateStateMachine<StateMachine>(resolver);

            await stateMachine.Execute<FooState>(CancellationToken.None);
        }

        public void Clear()
        {
            // UniState clears resources automatically
        }
    }
}
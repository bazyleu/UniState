using System.Threading;
using Benchmarks.Common;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace Benchmarks.UniStateFixtures
{
    public class UniBenchmarkRunner : MonoBehaviour
    {
        private void Start()
        {
            Run(5, false).Forget();
        }

        public async UniTask Run(int stateCount, bool withoutHistory)
        {
            var benchmarkHelper = new BenchmarkHelper();
            var resolver = new SimpleResolver(benchmarkHelper);

            benchmarkHelper.SetStatesCountTarget(stateCount);

            IExecutableStateMachine stateMachine =
                withoutHistory
                    ? StateMachineHelper.CreateStateMachine<StateMachineWithoutHistory>(resolver)
                    : StateMachineHelper.CreateStateMachine<StateMachine>(resolver);

            await stateMachine.Execute<FooState>(CancellationToken.None);

            Debug.Log("UniBenchmarkRunner - ExecutedMethods:" + benchmarkHelper.ExecutedMethods);
        }
    }
}
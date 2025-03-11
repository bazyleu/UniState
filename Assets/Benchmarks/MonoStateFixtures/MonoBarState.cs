using UnityEngine;

namespace Benchmarks.MonoStateFixtures
{
    public class MonoBarState : MonoStateBase
    {
        protected override MonoStateBase GetNextState()
        {
            var state = new GameObject().AddComponent<MonoFooState>();
            state.SetBenchmarkHelper(BenchmarkHelper);

            return state;
        }
    }
}
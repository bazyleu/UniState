using UnityEngine;

namespace Benchmarks.MonoStateFixtures
{
    public class MonoFooState : MonoStateBase
    {
        protected override MonoStateBase GetNextState()
        {
            var state = new GameObject().AddComponent<MonoBarState>();
            state.SetBenchmarkHelper(BenchmarkHelper);

            return state;
        }
    }
}
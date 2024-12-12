using System.Collections;
using Benchmarks.Common;
using UnityEngine;

namespace Benchmarks.MonoStateFixtures
{
    public abstract class MonoStateBase : MonoBehaviour
    {
        protected BenchmarkHelper BenchmarkHelper;

        public MonoStateBase NextState;

        public IEnumerator Initialize()
        {
            BenchmarkHelper.InitializeWasInvoked();

            yield return null;
        }

        public IEnumerator Execute()
        {
            NextState = GetNextState();

            BenchmarkHelper.ExecuteWasInvoked();

            if (BenchmarkHelper.TargetReached())
            {
                NextState = null;
            }

            yield return null;
        }

        public IEnumerator Exit()
        {
            BenchmarkHelper.ExitWasInvoked();

            yield return null;
        }

        public void SetBenchmarkHelper(BenchmarkHelper benchmarkHelper)
        {
            BenchmarkHelper = benchmarkHelper;
        }

        protected abstract MonoStateBase GetNextState();
    }
}
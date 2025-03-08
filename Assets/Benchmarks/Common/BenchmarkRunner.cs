using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace Benchmarks.Common
{
    public class BenchmarkRunner : MonoBehaviour
    {
        [SerializeField]
        private MonoBehaviour[] _benchmarkTests;

        private void Start()
        {
            var benchmarkTests = new List<IBenchmarkTest>(_benchmarkTests.Length);

            foreach (var test in _benchmarkTests)
            {
                if (test is IBenchmarkTest benchmarkTest)
                {
                    benchmarkTests.Add(benchmarkTest);
                }
            }

            _ = Run(benchmarkTests);
        }

        private async UniTaskVoid Run(List<IBenchmarkTest> benchmarkTests)
        {
            var results = new List<BenchmarkTestResult>(benchmarkTests.Count * 10);

            DisableGC();

            for (int i = 0; i < benchmarkTests.Count; i++)
            {
                var stopwatch = new Stopwatch();
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();

                stopwatch.Start();
                var test = benchmarkTests[i];

                long allocatedBefore = Profiler.GetMonoUsedSizeLong();

                await test.Run();

                long allocatedAfter = Profiler.GetMonoUsedSizeLong();

                test.Clear();
                stopwatch.Stop();

                var time = TimeSpan.FromTicks(stopwatch.Elapsed.Ticks);

                results.Add(new BenchmarkTestResult()
                {
                    TestName = test.Name,
                    ExecutionTimeSec = (int)time.TotalMilliseconds,
                    AllocatedMemoryBytes = allocatedAfter - allocatedBefore,
                });

                Debug.Log($"Benchmark {test.Name} completed. Run time: {(int)time.TotalMilliseconds}, allocated memory: {allocatedAfter - allocatedBefore}");
            }

            EnableGC();

            var averageResults = results.GroupBy(t => t.TestName).Select(g => new BenchmarkTestResult()
            {
                TestName = g.Key,
                ExecutionTimeSec = (int)g.Average(r => r.ExecutionTimeSec),
                AllocatedMemoryBytes = (int)g.Average(r => r.AllocatedMemoryBytes),
            });

            //TODO: Check and print results and averageResults + run 10 times
        }

        private void DisableGC()
        {
#if !UNITY_EDITOR
            GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
#endif
        }

        private void EnableGC()
        {
#if !UNITY_EDITOR
            GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
#endif
        }
    }
}
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
        private const int NumberOfCycles = 10;

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
            var results = new List<BenchmarkTestResult>(benchmarkTests.Count * NumberOfCycles);

            DisableGC();

            for (int i = 0; i < NumberOfCycles; i++)
            {
                for (int j = 0; j < benchmarkTests.Count; j++)
                {
                    var stopwatch = new Stopwatch();
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();

                    stopwatch.Start();
                    var test = benchmarkTests[j];

                    long allocatedBefore = Profiler.GetMonoUsedSizeLong();

                    await test.Run();

                    long allocatedAfter = Profiler.GetMonoUsedSizeLong();

                    test.Clear();
                    stopwatch.Stop();

                    var time = TimeSpan.FromTicks(stopwatch.Elapsed.Ticks);

                    results.Add(new BenchmarkTestResult()
                    {
                        TestName = test.Name,
                        ExecutionTimeMs = time.TotalMilliseconds,
                        AllocatedMemoryBytes = allocatedAfter - allocatedBefore,
                    });
                }
            }

            EnableGC();

            var averageResults = results.GroupBy(t => t.TestName).Select(g => new BenchmarkTestResult()
            {
                TestName = g.Key,
                ExecutionTimeMs = g.Average(r => r.ExecutionTimeMs),
                AllocatedMemoryBytes = (int)g.Average(r => r.AllocatedMemoryBytes),
            });

            var orderedResults = results.OrderBy(r => r.TestName).ThenByDescending(r => r.AllocatedMemoryBytes)
                .ToArray();

            Debug.Log($"+++ Benchmark Results Section +++");

            foreach (var result in orderedResults)
            {
                Debug.Log(result.ToString());
            }

            Debug.Log($"+++ Average Benchmark Results Section +++");

            foreach (var result in averageResults)
            {
                Debug.Log(result.ToString());
            }
        }

        private void DisableGC()
        {
#if !UNITY_EDITOR
            UnityEngine.Scripting.GarbageCollector.GCMode = UnityEngine.Scripting.GarbageCollector.Mode.Disabled;
#endif
        }

        private void EnableGC()
        {
#if !UNITY_EDITOR
            UnityEngine.Scripting.GarbageCollector.GCMode = UnityEngine.Scripting.GarbageCollector.Mode.Enabled;
#endif
        }
    }
}
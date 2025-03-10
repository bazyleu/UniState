namespace Benchmarks.Common
{
    public class BenchmarkTestResult
    {
        public string TestName;
        public double ExecutionTimeMs;
        public long AllocatedMemoryBytes;

        public override string ToString()
        {
            return $"Benchmark {TestName}: {ExecutionTimeMs:N1} ms, {(AllocatedMemoryBytes * 0.001f):N2} KB";
        }
    }
}
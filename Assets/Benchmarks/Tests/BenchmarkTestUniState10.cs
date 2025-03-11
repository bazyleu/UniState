using Benchmarks.Common;
using Benchmarks.UniStateFixtures;
using Cysharp.Threading.Tasks;

namespace Benchmarks.Tests
{
    public class BenchmarkTestUniState10 : UniBenchmarkTestBase, IBenchmarkTest
    {
        public string Name => "UniState 10 states";
        public UniTask Run() => Run(10, true);
    }
}
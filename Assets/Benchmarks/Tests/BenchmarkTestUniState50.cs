using Benchmarks.Common;
using Benchmarks.UniStateFixtures;
using Cysharp.Threading.Tasks;

namespace Benchmarks.Tests
{
    public class BenchmarkTestUniState50 : UniBenchmarkTestBase, IBenchmarkTest
    {
        public string Name => "UniState 50 states";
        public UniTask Run() => Run(50, true);
    }
}
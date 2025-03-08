using Benchmarks.Common;
using Benchmarks.UniStateFixtures;
using Cysharp.Threading.Tasks;

namespace Benchmarks.Tests
{
    public class BenchmarkTestUniState200 : UniBenchmarkTestBase, IBenchmarkTest
    {
        public string Name => "UniState 200 states";
        public UniTask Run() => Run(200, true);
    }
}
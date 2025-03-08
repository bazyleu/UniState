using Benchmarks.Common;
using Benchmarks.UniStateFixtures;
using Cysharp.Threading.Tasks;

namespace Benchmarks.Tests
{
    public class BenchmarkTestUniStateHistory200 : UniBenchmarkTestBase, IBenchmarkTest
    {
        public string Name => "UniState with history 200 states";
        public UniTask Run() => Run(200, false);
    }
}
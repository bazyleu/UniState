using Benchmarks.Common;
using Benchmarks.UniStateFixtures;
using Cysharp.Threading.Tasks;

namespace Benchmarks.Tests
{
    public class BenchmarkTestUniStateHistory10 : UniBenchmarkTestBase, IBenchmarkTest
    {
        public string Name => "UniState with history 10 states";
        public UniTask Run() => Run(10, false);
    }
}
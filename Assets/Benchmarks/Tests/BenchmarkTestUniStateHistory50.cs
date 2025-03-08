using Benchmarks.Common;
using Benchmarks.UniStateFixtures;
using Cysharp.Threading.Tasks;

namespace Benchmarks.Tests
{
    public class BenchmarkTestUniStateHistory50 : UniBenchmarkTestBase, IBenchmarkTest
    {
        public string Name => "UniState with history 50 states";
        public UniTask Run() => Run(50, false);
    }
}
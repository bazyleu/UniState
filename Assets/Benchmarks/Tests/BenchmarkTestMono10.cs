using Benchmarks.Common;
using Benchmarks.MonoStateFixtures;
using Cysharp.Threading.Tasks;

namespace Benchmarks.Tests
{
    public class BenchmarkTestMono10 : MonoBenchmarkTestBase, IBenchmarkTest
    {
        public string Name => "Mono 10 states";
        public UniTask Run() => Run(10);
    }
}
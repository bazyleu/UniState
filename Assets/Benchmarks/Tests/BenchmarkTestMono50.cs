using Benchmarks.Common;
using Benchmarks.MonoStateFixtures;
using Cysharp.Threading.Tasks;

namespace Benchmarks.Tests
{
    public class BenchmarkTestMono50 : MonoBenchmarkTestBase, IBenchmarkTest
    {
        public string Name => "Mono 50 states";
        public UniTask Run() => Run(50);
    }
}
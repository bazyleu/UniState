using Benchmarks.Common;
using Benchmarks.MonoStateFixtures;
using Cysharp.Threading.Tasks;

namespace Benchmarks.Tests
{
    public class BenchmarkTestMono200 : MonoBenchmarkTestBase, IBenchmarkTest
    {
        public string Name => "Mono 200 states";
        public UniTask Run() => Run(200);
    }
}
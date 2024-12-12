using Cysharp.Threading.Tasks;

namespace Benchmarks.Common
{
    public interface IBenchmarkTest
    {
        string Name { get; }
        UniTask Run();
    }
}
using BenchmarkDotNet.Running;
using UniState;


var summary = BenchmarkRunner.Run<BenchmarkTests>();

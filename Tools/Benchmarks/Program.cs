#region
using BenchmarkDotNet.Running;
#endregion

namespace Benchmarks;

public static class Program
{
    public static void Main() => BenchmarkRunner.Run<LockBenchmarks>();
}
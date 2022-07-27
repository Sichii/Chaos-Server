using BenchmarkDotNet.Attributes;
using Chaos.Core.Synchronization;

// ReSharper disable NotAccessedVariable

namespace Benchmarks;

[MemoryDiagnoser]
public class LockBenchmarks
{
    private readonly object Sync = new object();
    private readonly AutoReleasingMonitor Auto = new AutoReleasingMonitor();

    [Params(100, 1000, 10000)]
    public int NumRuns { get; set; }

    [Benchmark(Baseline = true)]
    public void NormalLock()
    {
        var random = Random.Shared.Next(0, 20);
        
        for (var i = 0; i < NumRuns; i++)
            lock (Sync)
                random += Random.Shared.Next(0, 20);
    }

    [Benchmark]
    public void AutoLock()
    {
        var random = Random.Shared.Next(0, 20);
        
        for (var i = 0; i < NumRuns; i++)
        {
            using var @lock = Auto.Enter();
            random += Random.Shared.Next(0, 20);
        }
    }
}
using BenchmarkDotNet.Attributes;
using Chaos.Common.Synchronization;

// ReSharper disable NotAccessedVariable

namespace Benchmarks;

[MemoryDiagnoser]

// ReSharper disable once ClassCanBeSealed.Global
public class LockBenchmarks
{
    private readonly AutoReleasingMonitor AutoMonitor = new();
    private readonly AutoReleasingReaderWriterLockSlim AutoReaderWriterLockSlim = new();
    private readonly object Sync = new();

    public int NumRuns { get; set; }

    [Benchmark]
    public void AutoLock()
    {
        using var @lock = AutoMonitor.Enter();
    }

    [Benchmark]
    public void AutoReaderWriterLock()
    {
        using var @lock = AutoReaderWriterLockSlim.EnterReadLock();
    }

    [Benchmark(Baseline = true)]
    public void NormalLock()
    {
        lock (Sync) { }
    }
}
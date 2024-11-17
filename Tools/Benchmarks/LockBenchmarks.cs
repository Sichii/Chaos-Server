#region
using BenchmarkDotNet.Attributes;
using Chaos.Common.Synchronization;
#endregion

#pragma warning disable CS0618 // Type or member is obsolete

// ReSharper disable NotAccessedVariable

namespace Benchmarks;

[MemoryDiagnoser]

// ReSharper disable once ClassCanBeSealed.Global
public class LockBenchmarks
{
    private readonly AutoReleasingMonitor AutoMonitor = new();
    private readonly AutoReleasingReaderWriterLockSlim AutoReaderWriterLockSlim = new();
    private readonly Lock NewLock = new();

    // ReSharper disable once ChangeFieldTypeToSystemThreadingLock
    private readonly object Sync = new();

    public int NumRuns { get; set; }

    [Benchmark]
    public void AutoMonitorLock()
    {
        using var @lock = AutoMonitor.Enter();
    }

    [Benchmark]
    public void AutoNewLock()
    {
        using var @lock = NewLock.EnterScope();
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
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
// ReSharper disable once ClassCanBeSealed.Global
public class ForEachBenchmark
{
    public int[] Array { get; set; } = null!;
    public List<int> Collection { get; set; } = null!;
    [Params(
        100,
        1_000,
        10_000,
        100_000)]
    public int CollectionSize { get; set; }

    [Benchmark(Baseline = true)]
    public void ForArray()
    {
        // ReSharper disable once NotAccessedVariable
        var num = 0;

        for (var i = 0; i < Array.Length; i++)
            num += Array[i];
    }

    [Benchmark]
    public void ForEach()
    {
        // ReSharper disable once NotAccessedVariable
        var num = 0;

        foreach (var obj in Collection)
            num += obj;
    }

    [Benchmark]
    public void ForEachSpan()
    {
        // ReSharper disable once NotAccessedVariable
        var num = 0;

        foreach (ref var obj in CollectionsMarshal.AsSpan(Collection))
            num += obj;
    }

    [Benchmark]
    public void ForSpan()
    {
        // ReSharper disable once NotAccessedVariable
        var num = 0;
        var span = Array.AsSpan();

        for (var i = 0; i < span.Length; i++)
            num += span[i];
    }

    [GlobalSetup]
    public void Setup()
    {
        Collection = Enumerable.Range(0, CollectionSize).ToList();
        Array = Collection.ToArray();
    }
}
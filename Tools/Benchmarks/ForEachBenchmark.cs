using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
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

    [Benchmark]
    public void ForEachSpan()
    {
        var num = 0;
        
        foreach (ref var obj in CollectionsMarshal.AsSpan(Collection))
            num += obj;
    }

    [Benchmark]
    public void ForSpan()
    {
        var num = 0;
        var span = Array.AsSpan();

        for(var i = 0; i < span.Length; i++)
            num += span[i];
    }

    [Benchmark]
    public void ForEach()
    {
        var num = 0;
        
        foreach (var obj in Collection)
            num += obj;
    }

    [Benchmark(Baseline = true)]
    public void ForArray()
    {
        var num = 0;
        
        for (var i = 0; i < Array.Length; i++)
            num += Array[i];
    }
    
    [GlobalSetup]
    public void Setup()
    {
        Collection = Enumerable.Range(0, CollectionSize).ToList();
        Array = Collection.ToArray();
    }
}
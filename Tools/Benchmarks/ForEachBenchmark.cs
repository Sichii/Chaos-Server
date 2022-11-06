using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class ForEachBenchmark
{
    public int[] Array { get; set; } = null!;
    public List<int> Collection { get; set; } = null!;
    [Params(
        1,
        10,
        100,
        1000,
        10000)]
    public int CollectionSize { get; set; }

    [Benchmark]
    public void ExplcitFastForEach()
    {
        foreach (var obj in CollectionsMarshal.AsSpan(Collection))
        {
            var newNum = obj + 5;
        }
    }

    [Benchmark]
    public void NormalForEach()
    {
        foreach (var obj in Collection)
        {
            var newNum = obj + 5;
        }
    }

    [Benchmark(Baseline = true)]
    public void NormalForEachArray()
    {
        for (var i = 0; i < Array.Length; i++)
        {
            var newNum = Array[i] + 5;
        }
    }

    private void PerformAction(int num)
    {
        var newNum = num + 5;
    }

    [GlobalSetup]
    public void Setup()
    {
        Collection = Enumerable.Range(0, CollectionSize).ToList();
        Array = Collection.ToArray();
    }
}
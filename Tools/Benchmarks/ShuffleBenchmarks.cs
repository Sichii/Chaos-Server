using BenchmarkDotNet.Attributes;
using Chaos.Extensions.Common;

namespace Benchmarks;

[MemoryDiagnoser]
// ReSharper disable once ClassCanBeSealed.Global
public class ShuffleBenchmark
{
    public int[] Array { get; } = Enumerable.Range(0, 100).Select(_ => Random.Shared.Next()).ToArray();
    public int[] RandomizedIndexes { get; } = Enumerable.Range(0, 100).Select(i => i).ToArray();

    [Benchmark(Baseline = true)]
    public void OrderByShuffle()
    {
        foreach (var _ in Array.OrderBy(_ => Random.Shared.Next())) { }
    }

    [Benchmark]
    public void RandomShuffle()
    {
        RandomizedIndexes.Shuffle();

        for (var i = 0; i < RandomizedIndexes.Length; i++)
        {
            var _ = Array[RandomizedIndexes[i]];
        }
    }

    [Benchmark]
    public void ToListShuffle()
    {
        // ReSharper disable once UnusedVariable
        var list = Array.AsEnumerable().Shuffle();
    }
}
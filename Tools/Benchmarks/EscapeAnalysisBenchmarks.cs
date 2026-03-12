#region
using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Chaos.Extensions.Common;
#endregion

namespace Benchmarks;

[MemoryDiagnoser]
[MaxIterationCount(50)]
[MaxWarmupCount(50)]

// ReSharper disable once ClassCanBeSealed.Global
public class EscapeAnalysisBenchmarks
{
    private Consumer Consumer;

    [Params(
        1,
        10,
        100,
        1000,
        10_000)]
    public int Count;

    private Random Random;

    [Benchmark]
    public void BlindRentedLine()
    {
        using var rented = RandomIterator()
            .ToRented();
        var span = rented.Span;

        for (var i = 0; i < span.Length; i++)
            Consumer.Consume(span[i]);
    }

    [Benchmark]
    public void OptimalLine()
    {
        Span<int> span = stackalloc int[Count];
        var i = 0;

        foreach (var item in RandomIterator())
            span[i++] = item;

        for (i = 0; i < span.Length; i++)
            Consumer.Consume(span[i]);
    }

    public IEnumerable<int> RandomIterator()
    {
        foreach (var item in Enumerable.Range(0, Count)
                                       .Select(_ => Random.Next()))
            yield return item;
    }

    [Benchmark]
    public void RentedLine()
    {
        using var rented = RandomIterator()
            .ToRented(Count);
        var span = rented.Span;

        for (var i = 0; i < span.Length; i++)
            Consumer.Consume(span[i]);
    }

    [GlobalSetup]
    public void Setup()
    {
        Consumer = new Consumer();
        Random = new Random(12345);

        using var warmup = MemoryPool<int>.Shared.Rent(Count);
    }

    [Benchmark(Baseline = true)]
    public void ToArray()
    {
        var localArray = RandomIterator()
            .ToArray();

        for (var i = 0; i < localArray.Length; i++)
            Consumer.Consume(localArray[i]);
    }
}
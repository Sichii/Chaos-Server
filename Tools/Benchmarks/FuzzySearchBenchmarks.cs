using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using Chaos.Extensions.Common;

// ReSharper disable ClassCanBeSealed.Global

[assembly: SuppressMessage("ReSharper", "ClassCanBeSealed.Global")]

namespace Benchmarks;

[MemoryDiagnoser]
public class FuzzySearchBenchmarks
{
    public const string LONG_STRING_1 = "The quick brown fox jumps over the lazy dog";
    public const string LONG_STRING_2 = "The quickest brownest fox jumps over the laziest dog";
    public const string SHORT_STRING_1 = "Bats Wing";
    public const string SHORT_STRING_2 = "Bat's Wing";

    /*
    [Benchmark]
    public void DiceCoefficientLongString_2()
    {
        var result = LONG_STRING_1.CalculateDiceCoefficient(LONG_STRING_2);

        if (result < 0.5m)
            throw new InvalidOperationException($"{result} is probably not correct");
    }

    [Benchmark]
    public void DiceCoefficientShortString_2()
    {
        var result = SHORT_STRING_1.CalculateDiceCoefficient(SHORT_STRING_2);

        if (result < 0.5m)
            throw new InvalidOperationException($"{result} is probably not correct");
    }*/

    [Benchmark]
    public void DistanceLongString_2()
    {
        var result = LONG_STRING_1.CalculateLevenshteinDistance(LONG_STRING_2);

        if (result != 10)
            throw new InvalidOperationException($"{result} is probably not correct");
    }

    [Benchmark]
    public void DistanceShortString_2()
    {
        var result = SHORT_STRING_1.CalculateLevenshteinDistance(SHORT_STRING_2);

        if (result != 1)
            throw new InvalidOperationException($"{result} is probably not correct");
    }
}
using BenchmarkDotNet.Attributes;
using Chaos.Extensions.Common;

// ReSharper disable All

namespace Benchmarks;

[MemoryDiagnoser]
// ReSharper disable once ClassCanBeSealed.Global
public class StringProcessorBenchmarks
{
    private const string LONG_STRING_FORMAT = MEDIUM_STRING_FORMAT + "  " + MEDIUM_STRING_FORMAT;
    private const string LONG_STRING_VALUE = MEDIUM_STRING_VALUE + "  " + MEDIUM_STRING_VALUE;
    private const string MEDIUM_STRING_FORMAT = "One is {0}, {{Two}} is {1}, {{{{Three}}}} is {{Three}}, {2} is {{{{Four}}}}, {{Tip}}";
    private const string MEDIUM_STRING_VALUE = "One is 1, {Two} is 2, {{Three}} is {Three}, 4 is {{Four}}, {Tip}";
    private const string SHORT_STRING = "One is {One}, {{Two}} is {Two}";
    private const string SHORT_STRING_FORMAT = "One is {0}, {{Two}} is {1}";
    private const string SHORT_STRING_VALUE = "One is 1, {Two} is 2";

    [Benchmark]
    public void FormatLongString()
    {
        var str = string.Format(
            LONG_STRING_FORMAT,
            1,
            2,
            4);

        if (!str.Equals(LONG_STRING_VALUE))
            throw new InvalidOperationException($"{str} is not {LONG_STRING_VALUE}");
    }

    [Benchmark]
    public void FormatMediumString()
    {
        var str = string.Format(
            MEDIUM_STRING_FORMAT,
            1,
            2,
            4);

        if (!str.Equals(MEDIUM_STRING_VALUE))
            throw new InvalidOperationException($"{str} is not {MEDIUM_STRING_VALUE}");
    }

    [Benchmark(Baseline = true)]
    public void FormatShortString()
    {
        var str = string.Format(SHORT_STRING_FORMAT, 1, 2);

        if (!str.Equals(SHORT_STRING_VALUE))
            throw new InvalidOperationException($"{str} is not {SHORT_STRING_VALUE}");
    }

    [Benchmark]
    public void Inject2LongString()
    {
        var str =
            LONG_STRING_FORMAT.Inject(
                1,
                2,
                4,
                1,
                2,
                4);

        if (!str.Equals(LONG_STRING_VALUE))
            throw new InvalidOperationException($"{str} is not {LONG_STRING_VALUE}");
    }

    [Benchmark]
    public void Inject2MediumString()
    {
        var str =
            MEDIUM_STRING_FORMAT.Inject(
                1,
                2,
                4);

        if (!str.Equals(MEDIUM_STRING_VALUE))
            throw new InvalidOperationException($"{str} is not {MEDIUM_STRING_VALUE}");
    }

    [Benchmark]
    public void Inject2ShortString()
    {
        var str = SHORT_STRING.Inject(1, 2);

        if (!str.Equals(SHORT_STRING_VALUE))
            throw new InvalidOperationException($"{str} is not {SHORT_STRING_VALUE}");
    }
}
#region
using BenchmarkDotNet.Attributes;
using Chaos.Extensions.Common;
#endregion

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
    public string FormatLongString()
        => string.Format(
            LONG_STRING_FORMAT,
            1,
            2,
            4);

    [Benchmark]
    public string FormatMediumString()
        => string.Format(
            MEDIUM_STRING_FORMAT,
            1,
            2,
            4);

    [Benchmark(Baseline = true)]
    public string FormatShortString() => string.Format(SHORT_STRING_FORMAT, 1, 2);

    [Benchmark]
    public string InjectLongString()
        => LONG_STRING_FORMAT.Inject(
            [
                1,
                2,
                4,
                1,
                2,
                4
            ]);

    [Benchmark]
    public string InjectMediumString()
        => MEDIUM_STRING_FORMAT.Inject(
            [
                1,
                2,
                4
            ]);

    [Benchmark]
    public string InjectShortString()
        => SHORT_STRING.Inject(
            [
                1,
                2
            ]);
}
using BenchmarkDotNet.Attributes;
using Chaos.Extensions.Common;

namespace Benchmarks;

[MemoryDiagnoser]
public class StringProcessorBenchmarks
{
    private const string LongStringValue = MediumStringValue + "  " + MediumStringValue;
    private const string MediumStringValue = "One is 1, {Two} is 2, {{Three}} is {NotThree}, 4 is {{Four}}, {Tip}";
    private const string ShortStringValue = "One is 1, {Two} is 2";
    private readonly string MediumString = "One is {One}, {{Two}} is {Two}, {{{{Three}}}} is {{NotThree}}, {Four} is {{{{Four}}}}, {{Tip}}";
    private readonly string MediumStringFormat = "One is {0}, {{Two}} is {1}, {{{{Three}}}} is {{NotThree}}, {2} is {{{{Four}}}}, {{Tip}}";
    private readonly string ShortString = "One is {One}, {{Two}} is {Two}";
    private readonly string ShortStringFormat = "One is {0}, {{Two}} is {1}";
    private string LongString => MediumString + "  " + MediumString;
    private string LongStringFormat => MediumStringFormat + "  " + MediumStringFormat;

    [Benchmark]
    public void FormatLongString()
    {
        var str = string.Format(
            LongStringFormat,
            1,
            2,
            4,
            1,
            2,
            4);

        if (!str.Equals(LongStringValue))
            throw new InvalidOperationException($"{str} is not {LongStringValue}");
    }

    [Benchmark]
    public void FormatMediumString()
    {
        var str = string.Format(
            MediumStringFormat,
            1,
            2,
            4);

        if (!str.Equals(MediumStringValue))
            throw new InvalidOperationException($"{str} is not {MediumStringValue}");
    }

    [Benchmark(Baseline = true)]
    public void FormatShortString()
    {
        var str = string.Format(ShortStringFormat, 1, 2);

        if (!str.Equals(ShortStringValue))
            throw new InvalidOperationException($"{str} is not {ShortStringValue}");
    }

    [Benchmark]
    public void ProcessLongString()
    {
        var str =
            LongStringFormat.Inject(
                1,
                2,
                4,
                1,
                2,
                4);

        if (!str.Equals(LongStringValue))
            throw new InvalidOperationException($"{str} is not {LongStringValue}");
    }

    [Benchmark]
    public void ProcessMediumString()
    {
        var str =
            MediumStringFormat.Inject(
                1,
                2,
                4);

        if (!str.Equals(MediumStringValue))
            throw new InvalidOperationException($"{str} is not {MediumStringValue}");
    }

    [Benchmark]
    public void ProcessShortString()
    {
        var str = ShortString.Inject(1, 2);

        if (!str.Equals(ShortStringValue))
            throw new InvalidOperationException($"{str} is not {ShortStringValue}");
    }
}
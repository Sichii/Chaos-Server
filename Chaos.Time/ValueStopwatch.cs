using System.Diagnostics;

namespace Chaos.Time;

public readonly struct ValueStopwatch
{
    private static readonly double TIMESTAMP_TO_TICKS = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

    private readonly long StartTimeStamp;

    private ValueStopwatch(long startTimeStamp) => StartTimeStamp = startTimeStamp;

    public static TimeSpan GetElapsedTime(long startTimestamp, long endTimestamp)
    {
        var timestampDelta = endTimestamp - startTimestamp;
        var ticks = (long)(TIMESTAMP_TO_TICKS * timestampDelta);

        return new TimeSpan(ticks);
    }

    public TimeSpan GetElapsedTime() => GetElapsedTime(StartTimeStamp, GetTimestamp());

    public static long GetTimestamp() => Stopwatch.GetTimestamp();

    public static ValueStopwatch StartNew() => new(GetTimestamp());
}
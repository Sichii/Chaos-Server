using System.Diagnostics;

namespace Chaos.Time;

/// <summary>
///     A stopwatch alternative that does not allocate any memory
/// </summary>
public readonly ref struct ValueStopwatch
{
    private static readonly double TIMESTAMP_TO_TICKS = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

    private readonly long StartTimeStamp;

    private ValueStopwatch(long startTimeStamp) => StartTimeStamp = startTimeStamp;

    /// <summary>
    ///     Given two timestamps, calculates the amount of time between them
    /// </summary>
    /// <param name="startTimestamp">The starting time stamp</param>
    /// <param name="endTimestamp">The ending time stamp</param>
    /// <returns>A <see cref="System.TimeSpan"/> representing the amount of time between the two time stamps</returns>
    public static TimeSpan GetElapsedTime(long startTimestamp, long endTimestamp)
    {
        var timestampDelta = endTimestamp - startTimestamp;
        var ticks = (long)(TIMESTAMP_TO_TICKS * timestampDelta);

        return new TimeSpan(ticks);
    }

    /// <summary>
    ///     Gets the elapsed time since the stopwatch was started
    /// </summary>
    public TimeSpan GetElapsedTime() => GetElapsedTime(StartTimeStamp, GetTimestamp());
    
    /// <inheritdoc cref="System.Diagnostics.Stopwatch.GetTimestamp"/>
    public static long GetTimestamp() => Stopwatch.GetTimestamp();

    /// <summary>
    ///     Creates and starts a new <see cref="Chaos.Time.ValueStopwatch"/> instance
    /// </summary>
    public static ValueStopwatch StartNew() => new(GetTimestamp());
}
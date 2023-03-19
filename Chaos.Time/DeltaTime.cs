using System.Diagnostics;

namespace Chaos.Time;

/// <summary>
///     Provides an easy way to obtain a high-precision time-based delta value.
/// </summary>
public sealed class DeltaTime
{
    private long LastTimeStamp;

    /// <summary>
    ///     Gets the time elapsed since the last call to <see cref="GetDelta" />
    /// </summary>
    public TimeSpan GetDelta
    {
        get
        {
            var current = Stopwatch.GetTimestamp();
            var delta = Stopwatch.GetElapsedTime(LastTimeStamp, current);
            LastTimeStamp = current;

            return delta;
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeltaTime" /> class
    /// </summary>
    public DeltaTime() => LastTimeStamp = Stopwatch.GetTimestamp();
}
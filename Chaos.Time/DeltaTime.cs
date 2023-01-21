using System.Diagnostics;

namespace Chaos.Time;

/// <summary>
///     Provides an easy way to obtain a high-precision time-based delta value.
/// </summary>
public sealed class DeltaTime
{
    private long LastTimeStamp;

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

    public DeltaTime() => LastTimeStamp = Stopwatch.GetTimestamp();
}
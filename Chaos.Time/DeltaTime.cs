using System.Diagnostics;

namespace Chaos.Time;

/// <summary>
///     Provides an easy way to obtain a high-precision time-based delta value.
/// </summary>
public class DeltaTime
{
    private readonly Stopwatch Stopwatch = Stopwatch.StartNew();
    private long LastValue;

    /// <summary>
    ///     Gets the current time value in milliseconds.
    /// </summary>
    public long Elapsed
    {
        get
        {
            var delta = Stopwatch.ElapsedMilliseconds - LastValue;
            LastValue += delta;

            return delta;
        }
    }

    public TimeSpan ElapsedSpan => TimeSpan.FromMilliseconds(Elapsed);
}
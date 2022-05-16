namespace Chaos.Core.Time;

/// <summary>
///     Provides an easy way to obtain a high-precision time-based value.
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
            var newValue = Stopwatch.ElapsedMilliseconds - LastValue;
            LastValue += newValue;

            return newValue;
        }
    }

    public TimeSpan ElapsedSpan => TimeSpan.FromMilliseconds(Elapsed);
}
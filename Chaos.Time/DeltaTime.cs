namespace Chaos.Time;

/// <summary>
///     Provides an easy way to obtain a high-precision time-based delta value.
/// </summary>
public sealed class DeltaTime
{
    private long LastTimeStamp;

    /// <summary>
    ///     The most recent delta value calculated by <see cref="SetDelta"/>
    /// </summary>
    public TimeSpan DeltaSpan { get; private set; }

    public DeltaTime() => SetDelta();

    /// <summary>
    ///     Calculates the time delta between now and the last time this method was called
    /// </summary>
    public void SetDelta()
    {
        var currentTimeStamp = ValueStopwatch.GetTimestamp();
        DeltaSpan = ValueStopwatch.GetElapsedTime(LastTimeStamp, currentTimeStamp);
        LastTimeStamp = currentTimeStamp;
    }
}
using System.Diagnostics;

namespace Chaos.Time;

/// <summary>
///     Provides an easy way to obtain a high-precision time-based delta value.
/// </summary>
public sealed class DeltaTime
{
    private long LastTimeStamp;
    public TimeSpan DeltaSpan { get; private set; }

    public DeltaTime() => SetDelta();
    
    public void SetDelta()
    {
        var currentTimeStamp = ValueStopwatch.GetTimestamp();
        DeltaSpan = ValueStopwatch.GetElapsedTime(LastTimeStamp, currentTimeStamp);
        LastTimeStamp = currentTimeStamp;
    }
}
using Chaos.Time.Abstractions;

namespace Chaos.Time;

/// <summary>
///     A timer that utilizes a delta time value to increment elapsed time
/// </summary>
public class IntervalTimer : IIntervalTimer
{
    /// <inheritdoc />
    public bool IntervalElapsed { get; protected set; }

    /// <summary>
    ///     The amount of time that has elapsed since this timer crossed it's <see cref="Interval"/>
    /// </summary>
    protected TimeSpan Elapsed { get; set; }
    
    /// <summary>
    ///     The amount of time that must accumulate to set <see cref="IntervalElapsed"/> to true
    /// </summary>
    protected TimeSpan Interval { get; set; }

    public IntervalTimer(TimeSpan interval, bool startAsElapsed = true)
    {
        Interval = interval;

        if (startAsElapsed)
            Elapsed = interval;
    }
    
    /// <summary>
    ///     Resets the timer, setting the elapsed time to 0
    /// </summary>
    public virtual void Reset()
    {
        Elapsed = TimeSpan.Zero;
        IntervalElapsed = false;
    }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta)
    {
        IntervalElapsed = false;
        //add delta to elapsed
        Elapsed += delta;

        //if the interval has elapsed, subtract the interval, set intervalElapsed to true
        if (Elapsed >= Interval)
        {
            Elapsed -= Interval;
            IntervalElapsed = true;
        }
    }
}
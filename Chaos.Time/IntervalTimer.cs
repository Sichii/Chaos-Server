using Chaos.Time.Abstractions;

namespace Chaos.Time;

/// <summary>
///     A timer that utilizes a delta time value to increment elapsed time
/// </summary>
public class IntervalTimer : IIntervalTimer
{
    /// <summary>
    ///     The amount of time that has elapsed since this timer crossed it's <see cref="Interval" />
    /// </summary>
    protected TimeSpan Elapsed { get; set; }

    /// <summary>
    ///     The amount of time that must accumulate to set <see cref="IntervalElapsed" /> to true
    /// </summary>
    protected TimeSpan Interval { get; set; }

    /// <inheritdoc />
    public bool IntervalElapsed { get; protected set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IntervalTimer" /> class
    /// </summary>
    /// <param name="interval">
    ///     The interval between setting <see cref="IntervalElapsed" /> to true
    /// </param>
    /// <param name="startAsElapsed">
    ///     Whether or not to create the timer in an elapsed state
    /// </param>
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
    public virtual void SetOrigin(DateTime origin)
    {
        var addedTime = DateTime.UtcNow - origin;
        var remainder = new TimeSpan(addedTime.Ticks % Interval.Ticks);

        Elapsed = remainder;
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
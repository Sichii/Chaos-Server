using Chaos.Time.Abstractions;

namespace Chaos.Time;

public class IntervalTimer : IIntervalTimer
{
    public bool IntervalElapsed { get; protected set; }

    protected TimeSpan Elapsed { get; set; }
    protected TimeSpan Interval { get; set; }

    public IntervalTimer(TimeSpan interval, bool startAsElapsed = true)
    {
        Interval = interval;

        if (startAsElapsed)
            Elapsed = interval;
    }

    /// <inheritdoc />
    public virtual void Reset()
    {
        Elapsed = TimeSpan.Zero;
        IntervalElapsed = false;
    }

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
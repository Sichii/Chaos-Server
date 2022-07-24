using Chaos.Time.Interfaces;

namespace Chaos.Time;

public class IntervalTimer : IIntervalTimer
{
    public bool IntervalElapsed { get; protected set; }
    protected TimeSpan Elapsed { get; set; }
    protected TimeSpan Interval { get; set; }

    public IntervalTimer(TimeSpan interval) => Interval = interval;

    public virtual void Update(TimeSpan delta)
    {
        IntervalElapsed = false;

        //if the interval has elapsed, subtract the interval, set intervalElapsed to true
        if (Elapsed >= Interval)
        {
            Elapsed -= Interval;
            IntervalElapsed = true;
        }

        //add delta to elapsed
        Elapsed += delta;
    }
}
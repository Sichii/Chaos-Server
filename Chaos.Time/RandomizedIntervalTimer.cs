using Chaos.Common.Definitions;
using Chaos.Common.Utilities;

namespace Chaos.Time;

public class RandomizedIntervalTimer : IntervalTimer
{
    protected int MaxRandomizationPct { get; set; }

    protected TimeSpan RandomizedInterval { get; set; }

    protected RandomizationType Type { get; set; }

    public RandomizedIntervalTimer(
        TimeSpan interval,
        int maxRandomizationPct,
        RandomizationType type = RandomizationType.Balanced,
        bool startAsElapsed = true
    )
        : base(interval)
    {
        MaxRandomizationPct = maxRandomizationPct;
        Type = type;
        SetRandomizedInterval();

        if (startAsElapsed)
            Elapsed = RandomizedInterval;
    }

    /// <inheritdoc />
    public override void Reset()
    {
        base.Reset();

        SetRandomizedInterval();
    }

    protected void SetRandomizedInterval()
    {
        var ticks = Interval.Ticks;
        var randomizedTicks = Randomizer.RollRange(ticks, MaxRandomizationPct, Type);

        RandomizedInterval = new TimeSpan(randomizedTicks);
    }

    public override void Update(TimeSpan delta)
    {
        IntervalElapsed = false;
        //add delta to elapsed
        Elapsed += delta;

        //if the interval has elapsed, subtract the interval, set intervalElapsed to true
        if (Elapsed >= RandomizedInterval)
        {
            Elapsed -= RandomizedInterval;
            IntervalElapsed = true;
            SetRandomizedInterval();
        }
    }
}
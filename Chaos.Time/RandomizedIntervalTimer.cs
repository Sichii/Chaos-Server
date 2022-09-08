using Chaos.Time.Definitions;

namespace Chaos.Time;

public class RandomizedIntervalTimer : IntervalTimer
{
    protected int MaxRandomizationPct { get; set; }

    protected TimeSpan RandomizedInterval { get; set; }

    protected RandomizationType Type { get; set; }

    public RandomizedIntervalTimer(TimeSpan interval, int maxRandomizationPct, RandomizationType type = RandomizationType.Balanced)
        : base(interval)
    {
        MaxRandomizationPct = maxRandomizationPct;
        Type = type;
        SetRandomizedInterval();
    }

    protected void SetRandomizedInterval()
    {
        var randomPct = Random.Shared.Next(0, MaxRandomizationPct);
        decimal applicablePct;

        switch (Type)
        {
            case RandomizationType.Balanced:
            {
                var half = MaxRandomizationPct / 2;

                applicablePct = (randomPct - half) / 100m;

                break;
            }
            case RandomizationType.Positive:
            {
                applicablePct = randomPct / 100m;

                break;
            }
            case RandomizationType.Negative:
            {
                applicablePct = -(randomPct / 100m);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        var amountToAdd = new TimeSpan((long)(Interval.Ticks * applicablePct));
        RandomizedInterval = Interval + amountToAdd;
    }
    
    /// <inheritdoc />
    public override void Reset()
    {
        base.Reset();
        
        SetRandomizedInterval();
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
namespace Chaos.Core.Time;

public class RandomizedIntervalTimer : IntervalTimer
{
    public enum RandomizationType
    {
        Balanced = 0,
        Positive = 1,
        Negative = 2
    }

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
        int applicablePct;

        switch (Type)
        {
            case RandomizationType.Balanced:
            {
                var half = MaxRandomizationPct / 2;

                applicablePct = (randomPct - half) / 100;

                break;
            }
            case RandomizationType.Positive:
            {
                applicablePct = randomPct / 100;

                break;
            }
            case RandomizationType.Negative:
            {
                applicablePct = -(randomPct / 100);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        var amountToAdd = Interval * applicablePct;
        RandomizedInterval = Interval + amountToAdd;
    }

    public override void Update(TimeSpan delta)
    {
        //if the interval has elapsed, subtract the interval, set intervalElapsed to true
        if (Elapsed >= RandomizedInterval)
        {
            Elapsed -= RandomizedInterval;
            IntervalElapsed = true;
            SetRandomizedInterval();
        }

        //add delta to elapsed
        Elapsed += delta;
    }
}
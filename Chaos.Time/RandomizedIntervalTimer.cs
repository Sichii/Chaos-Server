using Chaos.Common.Definitions;
using Chaos.Common.Utilities;

namespace Chaos.Time;

/// <summary>
///     A timers that utilizes a delta time value to increment elapsed time. Each time the interval elapses, a new interval
///     is set with an amount of randomization as specified through configuration
/// </summary>

// ReSharper disable once ClassCanBeSealed.Global
public class RandomizedIntervalTimer : IntervalTimer
{
    /// <summary>
    ///     The percent of randomization to apply to the <see cref="IntervalTimer.Interval" /> each time it elapses
    /// </summary>

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    protected int MaxRandomizationPct { get; set; }

    /// <summary>
    ///     The randomized interval that will be used until it elapses
    /// </summary>
    protected TimeSpan RandomizedInterval { get; set; }

    /// <summary>
    ///     The type of randomization to apply to the interval
    /// </summary>

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    protected RandomizationType Type { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RandomizedIntervalTimer" /> class
    /// </summary>
    /// <param name="interval">
    ///     The base interval to use and randomize
    /// </param>
    /// <param name="maxRandomizationPct">
    ///     The percent to randomize the base interval by
    /// </param>
    /// <param name="type">
    ///     The type of randomization to use
    /// </param>
    /// <param name="startAsElapsed">
    ///     Whether or not to create the timer in an elapsed state
    /// </param>
    public RandomizedIntervalTimer(
        TimeSpan interval,
        int maxRandomizationPct,
        RandomizationType type = RandomizationType.Balanced,
        bool startAsElapsed = true)
        : base(interval, startAsElapsed)
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

    /// <summary>
    ///     Sets the <see cref="RandomizedInterval" /> based on the <see cref="IntervalTimer.Interval" />,
    ///     <see cref="RandomizationType" />, and <see cref="MaxRandomizationPct" />
    /// </summary>
    protected void SetRandomizedInterval()
    {
        var ticks = Interval.Ticks;
        var randomizedTicks = IntegerRandomizer.RollRange(ticks, MaxRandomizationPct, Type);

        RandomizedInterval = new TimeSpan(randomizedTicks);
    }

    /// <inheritdoc />
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
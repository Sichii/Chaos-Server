#region
using Chaos.Common.Definitions;
using Chaos.Time;
using FluentAssertions;
#endregion

namespace Chaos.Time.Tests;

public sealed class RandomizedIntervalTimerTests
{
    [Test]
    public void Reset_ClearsElapsedAndAllowsNextCycle()
    {
        var timer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(100), 0, startAsElapsed: false);

        timer.Update(TimeSpan.FromMilliseconds(100));

        timer.IntervalElapsed
             .Should()
             .BeTrue();

        timer.Reset();

        timer.IntervalElapsed
             .Should()
             .BeFalse();

        // Half-tick should not yet elapse
        timer.Update(TimeSpan.FromMilliseconds(50));

        timer.IntervalElapsed
             .Should()
             .BeFalse();
    }

    [Test]
    public void Reset_MultipleResets_TimerContinuesToFunction()
    {
        var timer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(100), 0, startAsElapsed: false);

        for (var i = 0; i < 10; i++)
            timer.Reset();

        timer.Update(TimeSpan.FromMilliseconds(100));

        timer.IntervalElapsed
             .Should()
             .BeTrue();
    }

    [Test]
    public void StartAsElapsed_True_ElapseOnFirstZeroUpdate()
    {
        var timer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(100), 0, startAsElapsed: true);

        // Timer starts with elapsed pre-filled so a zero-delta update still elapses
        timer.Update(TimeSpan.Zero);

        timer.IntervalElapsed
             .Should()
             .BeTrue();
    }

    [Test]
    public void Update_Negative_RandomizationType_ReducesInterval()
    {
        // Negative type always reduces or keeps the interval at base * (1 - pct/100)
        var timer = new RandomizedIntervalTimer(
            TimeSpan.FromMilliseconds(200),
            50,
            RandomizationType.Negative,
            false);

        // The minimum possible interval is 100ms (50% of 200ms). Providing 200ms should always elapse.
        timer.Update(TimeSpan.FromMilliseconds(200));

        timer.IntervalElapsed
             .Should()
             .BeTrue();
    }

    [Test]
    public void Update_Positive_RandomizationType_IncreasesInterval()
    {
        // Positive type always increases or keeps the interval at base * (1 + pct/100)
        var timer = new RandomizedIntervalTimer(
            TimeSpan.FromMilliseconds(100),
            50,
            RandomizationType.Positive,
            false);

        // The maximum possible interval is 150ms (150% of 100ms). Providing 150ms should always elapse.
        timer.Update(TimeSpan.FromMilliseconds(150));

        timer.IntervalElapsed
             .Should()
             .BeTrue();
    }

    [Test]
    public void Update_WithRandomization_IntervalStillElapses()
    {
        // With up to 50% randomization, RandomizedInterval is at most 1.5x base.
        // Providing 1.5x base is guaranteed to elapse regardless of which direction randomization goes.
        var timer = new RandomizedIntervalTimer(
            TimeSpan.FromMilliseconds(100),
            50,
            RandomizationType.Balanced,
            false);

        timer.Update(TimeSpan.FromMilliseconds(150));

        timer.IntervalElapsed
             .Should()
             .BeTrue();
    }

    [Test]
    public void Update_WithZeroRandomization_ElapseAtExactInterval()
    {
        var timer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(100), 0, startAsElapsed: false);

        // One tick short of interval — should not elapse
        timer.Update(TimeSpan.FromMilliseconds(99));

        timer.IntervalElapsed
             .Should()
             .BeFalse();

        // One more tick crosses the interval
        timer.Update(TimeSpan.FromMilliseconds(1));

        timer.IntervalElapsed
             .Should()
             .BeTrue();
    }
}
#region
using FluentAssertions;
#endregion

namespace Chaos.Time.Tests;

public sealed class IntervalTimerTests
{
    [Test]
    public void Ctor_StartAsElapsedTrue_ShouldTriggerElapsedOnZeroUpdate()
    {
        var interval = TimeSpan.FromMilliseconds(100);
         var timer = new IntervalTimer(interval);

        timer.IntervalElapsed
             .Should()
             .BeFalse();
        timer.Update(TimeSpan.Zero);

        timer.IntervalElapsed
             .Should()
             .BeTrue();
    }

    [Test]
    public void Reset_ShouldClearElapsedFlag()
    {
        var timer = new IntervalTimer(TimeSpan.FromMilliseconds(10));

        timer.Update(TimeSpan.Zero);

        timer.IntervalElapsed
             .Should()
             .BeTrue();

        timer.Reset();

        timer.IntervalElapsed
             .Should()
             .BeFalse();

        timer.Update(TimeSpan.FromMilliseconds(9));

        timer.IntervalElapsed
             .Should()
             .BeFalse();
    }

    [Test]
    public void SetOrigin_ShouldSetRemainderSoNextUpdateCanElapse()
    {
        var interval = TimeSpan.FromMilliseconds(100);
         var timer = new IntervalTimer(interval, false);

        // Choose an origin very slightly in the past so remainder is tiny and the next update crosses the interval boundary
        var origin = DateTime.UtcNow - TimeSpan.FromMilliseconds(1);
        timer.SetOrigin(origin);

        timer.Update(TimeSpan.FromMilliseconds(100));

        timer.IntervalElapsed
             .Should()
             .BeTrue();
    }

    [Test]
    public void Update_EqualToInterval_ShouldElapseOnce()
    {
         var timer = new IntervalTimer(TimeSpan.FromMilliseconds(100), false);

        timer.Update(TimeSpan.FromMilliseconds(100));

        timer.IntervalElapsed
             .Should()
             .BeTrue();

        // Subsequent zero update should clear the flag until next interval reached
        timer.Update(TimeSpan.Zero);

        timer.IntervalElapsed
             .Should()
             .BeFalse();
    }

    [Test]
    public void Update_GreaterThanInterval_ShouldElapseOnceAndKeepRemainder()
    {
         var timer = new IntervalTimer(TimeSpan.FromMilliseconds(100), false);

        timer.Update(TimeSpan.FromMilliseconds(150));

        timer.IntervalElapsed
             .Should()
             .BeTrue();

        // Next small update should not immediately elapse again because only one interval is subtracted per update
        timer.Update(TimeSpan.FromMilliseconds(10));

        timer.IntervalElapsed
             .Should()
             .BeFalse();

        // Finishing the remainder should elapse again
        timer.Update(TimeSpan.FromMilliseconds(40));

        timer.IntervalElapsed
             .Should()
             .BeTrue();
    }

    [Test]
    public void Update_LessThanInterval_ShouldNotElapse()
    {
         var timer = new IntervalTimer(TimeSpan.FromMilliseconds(100), false);

        timer.Update(TimeSpan.FromMilliseconds(99));

        timer.IntervalElapsed
             .Should()
             .BeFalse();
    }
}
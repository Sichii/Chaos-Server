#region
using FluentAssertions;
using Moq;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Time.Tests;

public sealed class SequentialTimersTests
{
    [Test]
    public void PeriodicSequentialEventTimer_ShouldAlwaysUpdateFirstTimer()
    {
        var t1 = new Mock<IIntervalTimer>();
        var t2 = new Mock<IIntervalTimer>();

        t1.SetupGet(x => x.IntervalElapsed)
          .Returns(false);

        t2.SetupGet(x => x.IntervalElapsed)
          .Returns(true);

        var seq = new PeriodicSequentialEventTimer(t1.Object, t2.Object);

        seq.Update(TimeSpan.FromMilliseconds(1));

        t1.Verify(x => x.Update(It.IsAny<TimeSpan>()), Times.Exactly(1));
    }

    [Test]
    public void PeriodicSequentialEventTimer_WhenFirstIsNotCurrent_ShouldStillUpdateFirst()
    {
        var t1 = new Mock<IIntervalTimer>();
        var t2 = new Mock<IIntervalTimer>();

        t1.SetupGet(x => x.IntervalElapsed)
          .Returns(true); // Immediately advance to t2

        var seq = new PeriodicSequentialEventTimer(t1.Object, t2.Object);

        seq.Update(TimeSpan.FromMilliseconds(1));

        t1.Verify(x => x.Update(It.IsAny<TimeSpan>()), Times.AtLeastOnce());
        t2.Verify(x => x.Update(It.IsAny<TimeSpan>()), Times.Once);
    }

    [Test]
    public void RandomizedIntervalTimer_ShouldElapseAndRandomizeNextInterval()
    {
        // Use deterministic interval by setting randomization pct to 0
        var timer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(100), 0, startAsElapsed: false);

        timer.Update(TimeSpan.FromMilliseconds(100));

        timer.IntervalElapsed
             .Should()
             .BeTrue();

        // With remainder 0 and same interval, next zero update should not elapse
        timer.Update(TimeSpan.Zero);

        timer.IntervalElapsed
             .Should()
             .BeFalse();
    }

    [Test]
    public void SequentialEventTimer_ShouldAdvanceToNextTimer_WhenCurrentElapsed()
    {
        var t1 = new Mock<IIntervalTimer>();
        var t2 = new Mock<IIntervalTimer>();

        t1.SetupGet(x => x.IntervalElapsed)
          .Returns(true);

        var seq = new SequentialEventTimer(t1.Object, t2.Object);

        seq.Update(TimeSpan.FromMilliseconds(1));

        t2.Verify(x => x.Update(It.IsAny<TimeSpan>()), Times.Once);
    }
}
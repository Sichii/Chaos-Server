#region
using Chaos.Time.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Time.Tests;

public class SequentialEventTimerTests
{
    [Test]
    public void Reset_Should_Reset_All_Timers_And_Index()
    {
        var t1 = new FakeIntervalTimer(1);
        var t2 = new FakeIntervalTimer(2);

       var seq = new SequentialEventTimer(t1, t2);
        seq.Update(TimeSpan.FromMilliseconds(1)); // advance to t2

        seq.Reset();

        seq.CurrentTimer
           .Should()
           .BeSameAs(t1);

        seq.IntervalElapsed
           .Should()
           .BeFalse();
    }

    [Test]
    public void SetOrigin_Should_Throw_NotSupported()
    {
        var t1 = new FakeIntervalTimer(1);

       var seq = new SequentialEventTimer(t1);

        var act = () => seq.SetOrigin(DateTime.UtcNow);

        act.Should()
           .Throw<NotSupportedException>();
    }

    [Test]
    public void Update_Should_Advance_To_Next_Timer_When_Current_Elapsed()
    {
       var t1 = new FakeIntervalTimer(1);
       var t2 = new FakeIntervalTimer(2);

       var seq = new SequentialEventTimer(t1, t2);

        // first update should update t1; since t1 elapses immediately, next update moves to t2
        seq.Update(TimeSpan.FromMilliseconds(1));

        seq.CurrentTimer
           .Should()
           .BeSameAs(t1);
        seq.Update(TimeSpan.FromMilliseconds(1));

        seq.CurrentTimer
           .Should()
           .BeSameAs(t2);

        seq.IntervalElapsed
           .Should()
           .BeFalse();

        // two updates to elapse t2
        seq.Update(TimeSpan.FromMilliseconds(1));
        seq.Update(TimeSpan.FromMilliseconds(1));

        seq.IntervalElapsed
           .Should()
           .BeTrue();
    }

    [Test]
    public void Update_Should_Wrap_To_First_Timer_After_Last_Elapsed()
    {
        var t1 = new FakeIntervalTimer(1);
        var t2 = new FakeIntervalTimer(1);

       var seq = new SequentialEventTimer(t1, t2);

        // First update still on t1, second update moves to t2 (since t1 elapses)
        seq.Update(TimeSpan.FromMilliseconds(1));

        seq.CurrentTimer
           .Should()
           .BeSameAs(t1);
        seq.Update(TimeSpan.FromMilliseconds(1));

        seq.CurrentTimer
           .Should()
           .BeSameAs(t2);

        // Elapse t2 -> next update wraps and resets (single update)
        seq.Update(TimeSpan.FromMilliseconds(1));

        seq.CurrentTimer
           .Should()
           .BeSameAs(t1);

        // After wrap, timers should start fresh
        seq.Update(TimeSpan.FromMilliseconds(1));

        seq.CurrentTimer
           .Should()
           .BeSameAs(t2);
    }

    private sealed class FakeIntervalTimer : IIntervalTimer
    {
        private readonly int _ticksToElapse;
        private int _ticks;

        public bool IntervalElapsed => _ticks >= _ticksToElapse;

        public FakeIntervalTimer(int ticksToElapse)
        {
            _ticksToElapse = ticksToElapse;
            _ticks = 0;
        }

       public void Reset() => _ticks = 0;

        public void SetOrigin(DateTime origin)
        {
            // not used by tests
        }

       public void Update(TimeSpan delta) => _ticks++;
    }
}
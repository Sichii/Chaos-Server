#region
using Chaos.Time;
using Chaos.Time.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Time.Tests;

public sealed class PeriodicSequentialEventTimerTests
{
    [Test]
    public void Reset_RestartsCycleFromFirstTimer()
    {
        var t1 = new FakeIntervalTimer(1);
        var t2 = new FakeIntervalTimer(2);
        var seq = new PeriodicSequentialEventTimer(t1, t2);

        // Advance to t2
        seq.Update(TimeSpan.Zero); // t1 elapses
        seq.Update(TimeSpan.Zero); // advance to t2

        seq.CurrentTimer
           .Should()
           .BeSameAs(t2);

        seq.Reset();

        seq.CurrentTimer
           .Should()
           .BeSameAs(t1);

        seq.IntervalElapsed
           .Should()
           .BeFalse();
    }

    [Test]
    public void SetOrigin_Throws_NotSupportedException()
    {
        var t1 = new FakeIntervalTimer(1);
        var seq = new PeriodicSequentialEventTimer(t1);

        var act = () => seq.SetOrigin(DateTime.UtcNow);

        act.Should()
           .Throw<NotSupportedException>();
    }

    [Test]
    public void SingleTimer_CyclesBackAfterElapse()
    {
        var t1 = new FakeIntervalTimer(1);
        var seq = new PeriodicSequentialEventTimer(t1);

        // First update: update t1 → elapses
        seq.Update(TimeSpan.Zero);

        seq.CurrentTimer
           .Should()
           .BeSameAs(t1);

        seq.IntervalElapsed
           .Should()
           .BeTrue();

        // Second update: t1 elapsed → advance → overflow → Reset → update t1 → elapses again
        seq.Update(TimeSpan.Zero);

        seq.CurrentTimer
           .Should()
           .BeSameAs(t1);

        seq.IntervalElapsed
           .Should()
           .BeTrue();
    }

    [Test]
    public void Update_AdvancesToNextTimer_WhenCurrentElapsed()
    {
        var t1 = new FakeIntervalTimer(1);
        var t2 = new FakeIntervalTimer(2);
        var seq = new PeriodicSequentialEventTimer(t1, t2);

        seq.CurrentTimer
           .Should()
           .BeSameAs(t1);

        // First update: t1 not elapsed → update t1 → t1 elapses (1 tick)
        seq.Update(TimeSpan.Zero);

        seq.CurrentTimer
           .Should()
           .BeSameAs(t1);

        seq.IntervalElapsed
           .Should()
           .BeTrue();

        // Second update: t1 elapsed → advance to t2
        seq.Update(TimeSpan.Zero);

        seq.CurrentTimer
           .Should()
           .BeSameAs(t2);

        // t2 needs 2 ticks but only got 1 — not yet elapsed
        seq.IntervalElapsed
           .Should()
           .BeFalse();
    }

    [Test]
    public void Update_CyclesThroughAllTimers_AndResets()
    {
        var t1 = new FakeIntervalTimer(1);
        var t2 = new FakeIntervalTimer(1);
        var seq = new PeriodicSequentialEventTimer(t1, t2);

        // Update 1: update t1 → elapses
        seq.Update(TimeSpan.Zero);

        seq.CurrentTimer
           .Should()
           .BeSameAs(t1);

        // Update 2: t1 elapsed → advance to t2 → update t2 + t1(periodic) → t2 elapses
        seq.Update(TimeSpan.Zero);

        seq.CurrentTimer
           .Should()
           .BeSameAs(t2);

        seq.IntervalElapsed
           .Should()
           .BeTrue();

        // Update 3: t2 elapsed → advance → overflow → Reset → update t1 → elapses
        seq.Update(TimeSpan.Zero);

        seq.CurrentTimer
           .Should()
           .BeSameAs(t1);
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

        public void SetOrigin(DateTime origin) { }

        public void Update(TimeSpan delta) => _ticks++;
    }
}
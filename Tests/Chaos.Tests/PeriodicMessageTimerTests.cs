#region
using Chaos.Time;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class PeriodicMessageTimerTests
{
    #region Update — Already Elapsed
    [Test]
    public void Update_ShouldDoNothing_WhenIntervalAlreadyElapsed()
    {
        var messages = new List<string>();

        var timer = new PeriodicMessageTimer(
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(1),
            "{0} remaining",
            msg => messages.Add(msg));

        // Tick past total interval
        timer.Update(TimeSpan.FromSeconds(6));

        var messageCountAfterElapsed = messages.Count;

        // Further updates should be no-ops
        timer.Update(TimeSpan.FromSeconds(1));

        messages.Count
                .Should()
                .Be(messageCountAfterElapsed);
    }
    #endregion

    #region Update — Normal Sub-Interval Messages
    [Test]
    public void Update_ShouldSendMessage_WhenSubIntervalElapses()
    {
        var messages = new List<string>();

        // 30s total, 10s sub-interval, transition at 10s remaining, 2s transitioned interval
        var timer = new PeriodicMessageTimer(
            TimeSpan.FromSeconds(30),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(2),
            "{0} remaining",
            msg => messages.Add(msg));

        // Tick 10 seconds — sub-interval should fire
        timer.Update(TimeSpan.FromSeconds(10));

        messages.Should()
                .NotBeEmpty();
    }
    #endregion

    #region Update — Transitioned Interval Fires
    [Test]
    public void Update_ShouldSendMessage_WhenTransitionedIntervalElapses()
    {
        var messages = new List<string>();

        // 12s total, 5s sub-interval, transition at 5s remaining, 2s transitioned interval
        var timer = new PeriodicMessageTimer(
            TimeSpan.FromSeconds(12),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(2),
            "{0} remaining",
            msg => messages.Add(msg));

        // Tick to enter transition zone
        timer.Update(TimeSpan.FromSeconds(5));
        timer.Update(TimeSpan.FromSeconds(3));

        var messageCountBefore = messages.Count;

        // Tick 2s — transitioned interval should fire
        timer.Update(TimeSpan.FromSeconds(2));

        messages.Count
                .Should()
                .BeGreaterThan(messageCountBefore);
    }
    #endregion

    #region Update — Final Warning
    [Test]
    public void Update_ShouldSetFinalWarning_WhenCleanTimeSpanDropsBelowTransitionedInterval()
    {
        var messages = new List<string>();

        // 10s total, 3s sub-interval, transition at 4s remaining, 2s transitioned interval
        var timer = new PeriodicMessageTimer(
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(3),
            TimeSpan.FromSeconds(4),
            TimeSpan.FromSeconds(2),
            "{0} remaining",
            msg => messages.Add(msg));

        // Tick past sub-interval phase
        timer.Update(TimeSpan.FromSeconds(3));
        timer.Update(TimeSpan.FromSeconds(3));

        // Enter transition zone and tick through transitioned intervals
        timer.Update(TimeSpan.FromSeconds(2));
        timer.Update(TimeSpan.FromSeconds(2));

        var messageCountAfterFinal = messages.Count;

        // After final warning, further updates should not send more messages
        timer.Update(TimeSpan.FromSeconds(0.5));

        messages.Count
                .Should()
                .Be(messageCountAfterFinal);
    }
    #endregion

    #region Update — Transition to Faster Interval
    [Test]
    public void Update_ShouldTransitionToFasterInterval_WhenRemainingBelowThreshold()
    {
        var messages = new List<string>();

        // 20s total, 5s sub-interval, transition at 8s remaining, 2s transitioned interval
        var timer = new PeriodicMessageTimer(
            TimeSpan.FromSeconds(20),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(8),
            TimeSpan.FromSeconds(2),
            "{0} remaining",
            msg => messages.Add(msg));

        // Tick past the sub-interval phase (12s elapsed, 8s remaining = transition point)
        timer.Update(TimeSpan.FromSeconds(5));
        timer.Update(TimeSpan.FromSeconds(5));

        var messageCountBeforeTransition = messages.Count;

        // Now tick into transition zone (remaining < 8s)
        timer.Update(TimeSpan.FromSeconds(3));

        // Should have sent a transition message
        messages.Count
                .Should()
                .BeGreaterThan(messageCountBeforeTransition);
    }
    #endregion
}
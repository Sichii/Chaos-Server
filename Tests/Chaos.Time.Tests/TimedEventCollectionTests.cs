#region
using System.Collections;
using Chaos.Collections.Time;
using FluentAssertions;
#endregion

namespace Chaos.Time.Tests;

public sealed class TimedEventCollectionTests
{
    // The TimedEventCollection uses an IntervalTimer(1 second, startAsElapsed=true),
    // so the very first Update() call triggers the interval-elapsed cleanup branch.

    #region AddEvent
    [Test]
    public void AddEvent_EmptyEventId_ShouldThrowArgumentException()
    {
        var collection = new TimedEventCollection();

        var act = () => collection.AddEvent(string.Empty, TimeSpan.FromHours(1));

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void AddEvent_DuplicateNonAutoConsumeId_ShouldThrowInvalidOperationException()
    {
        var collection = new TimedEventCollection();
        collection.AddEvent("evt", TimeSpan.FromHours(1));

        var act = () => collection.AddEvent("evt", TimeSpan.FromHours(1));

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void AddEvent_DuplicateCompletedAutoConsumeId_ShouldReplace()
    {
        // Build a collection whose event is already completed (started 2 hours ago, duration 1h)
        var completed = new TimedEventCollection.Event(
            "evt",
            TimeSpan.FromHours(1),
            DateTime.UtcNow.AddHours(-2),
            true);
        var collection = new TimedEventCollection([completed]);

        // Adding the same ID again should succeed because the existing event is completed + autoConsume
        var act = () => collection.AddEvent("evt", TimeSpan.FromHours(1), true);

        act.Should()
           .NotThrow();
    }
    #endregion

    #region Constructor
    [Test]
    public void Constructor_WithInitialEvents_ShouldPopulateCollection()
    {
        var events = new[]
        {
            new TimedEventCollection.Event("event1", TimeSpan.FromHours(1)),
            new TimedEventCollection.Event("event2", TimeSpan.FromHours(2))
        };

        var collection = new TimedEventCollection(events);

        collection.HasActiveEvent("event1", out _)
                  .Should()
                  .BeTrue();

        collection.HasActiveEvent("event2", out _)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Constructor_WithoutEvents_ShouldCreateEmpty()
    {
        var collection = new TimedEventCollection();

        collection.HasActiveEvent("anything", out _)
                  .Should()
                  .BeFalse();
    }
    #endregion

    #region GetEnumerator
    [Test]
    public void GetEnumerator_Generic_ShouldEnumerateItems()
    {
        var collection = new TimedEventCollection();
        collection.AddEvent("key1", TimeSpan.FromHours(1));

        var items = new List<KeyValuePair<string, TimedEventCollection.Event>>();

        foreach (var kvp in collection)
            items.Add(kvp);

        items.Should()
             .HaveCount(1);
    }

    [Test]
    public void GetEnumerator_NonGeneric_ShouldEnumerateItems()
    {
        var collection = new TimedEventCollection();
        collection.AddEvent("key1", TimeSpan.FromHours(1));

        var enumerator = ((IEnumerable)collection).GetEnumerator();

        enumerator.MoveNext()
                  .Should()
                  .BeTrue();
    }
    #endregion

    #region HasActiveEvent
    [Test]
    public void HasActiveEvent_MissingKey_ShouldReturnFalse()
    {
        var collection = new TimedEventCollection();

        collection.HasActiveEvent("nonexistent", out _)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void HasActiveEvent_ActiveEvent_ShouldReturnTrue()
    {
        var collection = new TimedEventCollection();
        collection.AddEvent("active", TimeSpan.FromHours(1));

        var result = collection.HasActiveEvent("active", out var evt);

        result.Should()
              .BeTrue();

        evt!.EventId
            .Should()
            .Be("active");
    }

    [Test]
    public void HasActiveEvent_CompletedEvent_ShouldReturnFalse()
    {
        var completed = new TimedEventCollection.Event(
            "done",
            TimeSpan.FromHours(1),
            DateTime.UtcNow.AddHours(-2),
            false);
        var collection = new TimedEventCollection([completed]);

        collection.HasActiveEvent("done", out _)
                  .Should()
                  .BeFalse();
    }
    #endregion

    #region TryConsumeEvent
    [Test]
    public void TryConsumeEvent_MissingKey_ShouldReturnFalse()
    {
        var collection = new TimedEventCollection();

        collection.TryConsumeEvent("nonexistent", out _)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void TryConsumeEvent_ActiveEvent_ShouldReturnFalse()
    {
        var collection = new TimedEventCollection();
        collection.AddEvent("active", TimeSpan.FromHours(1));

        collection.TryConsumeEvent("active", out _)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void TryConsumeEvent_CompletedEvent_ShouldReturnTrueAndRemove()
    {
        var completed = new TimedEventCollection.Event(
            "done",
            TimeSpan.FromHours(1),
            DateTime.UtcNow.AddHours(-2),
            false);
        var collection = new TimedEventCollection([completed]);

        var result = collection.TryConsumeEvent("done", out var consumed);

        result.Should()
              .BeTrue();

        consumed!.EventId
                 .Should()
                 .Be("done");

        // The event should now be gone
        collection.TryConsumeEvent("done", out _)
                  .Should()
                  .BeFalse();
    }
    #endregion

    #region Update
    [Test]
    public void Update_IntervalElapsed_AutoConsumeCompleted_ShouldRemoveEvent()
    {
        var autoCompleted = new TimedEventCollection.Event(
            "auto",
            TimeSpan.FromHours(1),
            DateTime.UtcNow.AddHours(-2),
            true);
        var collection = new TimedEventCollection([autoCompleted]);

        // First Update triggers the elapsed branch (timer starts elapsed)
        collection.Update(TimeSpan.Zero);

        // Auto-consume + completed → removed
        collection.TryConsumeEvent("auto", out _)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void Update_IntervalElapsed_NonAutoConsumeCompleted_ShouldNotRemove()
    {
        var manualCompleted = new TimedEventCollection.Event(
            "manual",
            TimeSpan.FromHours(1),
            DateTime.UtcNow.AddHours(-2),
            false);
        var collection = new TimedEventCollection([manualCompleted]);

        collection.Update(TimeSpan.Zero);

        // Non-autoConsume events are NOT removed automatically
        collection.TryConsumeEvent("manual", out var evt)
                  .Should()
                  .BeTrue();

        evt!.EventId
            .Should()
            .Be("manual");
    }

    [Test]
    public void Update_IntervalElapsed_AutoConsumeActiveEvent_ShouldNotRemove()
    {
        var collection = new TimedEventCollection();
        collection.AddEvent("active", TimeSpan.FromHours(1), true);

        collection.Update(TimeSpan.Zero);

        // Active (not completed) auto-consume events should stay
        collection.HasActiveEvent("active", out _)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Update_SecondCall_NonElapsedInterval_ShouldNotTriggerCleanup()
    {
        var collection = new TimedEventCollection();
        collection.AddEvent("active", TimeSpan.FromHours(1));

        collection.Update(TimeSpan.Zero); // interval elapsed → cleanup (nothing to remove)
        collection.Update(TimeSpan.Zero); // interval not yet elapsed → no cleanup

        collection.HasActiveEvent("active", out _)
                  .Should()
                  .BeTrue();
    }
    #endregion

    #region Event inner class
    [Test]
    public void Event_Equals_NullOther_ShouldReturnFalse()
    {
        var evt = new TimedEventCollection.Event("test", TimeSpan.FromHours(1));

        evt.Equals(null)
           .Should()
           .BeFalse();
    }

    [Test]
    public void Event_Equals_SameReference_ShouldReturnTrue()
    {
        var evt = new TimedEventCollection.Event("test", TimeSpan.FromHours(1));

        evt.Equals(evt)
           .Should()
           .BeTrue();
    }

    [Test]
    public void Event_Equals_SameId_ShouldReturnTrue()
    {
        var evt1 = new TimedEventCollection.Event("test", TimeSpan.FromHours(1));
        var evt2 = new TimedEventCollection.Event("test", TimeSpan.FromHours(2));

        evt1.Equals(evt2)
            .Should()
            .BeTrue();
    }

    [Test]
    public void Event_Equals_DifferentId_ShouldReturnFalse()
    {
        var evt1 = new TimedEventCollection.Event("a", TimeSpan.FromHours(1));
        var evt2 = new TimedEventCollection.Event("b", TimeSpan.FromHours(1));

        evt1.Equals(evt2)
            .Should()
            .BeFalse();
    }

    [Test]
    public void Event_EqualsObject_Null_ShouldReturnFalse()
    {
        var evt = new TimedEventCollection.Event("test", TimeSpan.FromHours(1));

        evt.Equals((object?)null)
           .Should()
           .BeFalse();
    }

    [Test]
    public void Event_EqualsObject_SameReference_ShouldReturnTrue()
    {
        var evt = new TimedEventCollection.Event("test", TimeSpan.FromHours(1));

        evt.Equals((object)evt)
           .Should()
           .BeTrue();
    }

    [Test]
    public void Event_EqualsObject_NonEventType_ShouldReturnFalse()
    {
        var evt = new TimedEventCollection.Event("test", TimeSpan.FromHours(1));

        evt.Equals("some string")
           .Should()
           .BeFalse();
    }

    [Test]
    public void Event_EqualsObject_ValidEvent_ShouldReturnTrue()
    {
        var evt1 = new TimedEventCollection.Event("test", TimeSpan.FromHours(1));
        var evt2 = new TimedEventCollection.Event("test", TimeSpan.FromHours(2));

        evt1.Equals((object)evt2)
            .Should()
            .BeTrue();
    }

    [Test]
    public void Event_GetHashCode_ShouldBeConsistentForSameId()
    {
        var evt1 = new TimedEventCollection.Event("test", TimeSpan.FromHours(1));
        var evt2 = new TimedEventCollection.Event("test", TimeSpan.FromHours(2));

        evt1.GetHashCode()
            .Should()
            .Be(evt2.GetHashCode());
    }

    [Test]
    public void Event_Completed_ZeroDuration_ShouldBeTrue()
    {
        // Start is DateTime.UtcNow, Duration is 0 → Remaining ≈ 0 or negative
        var evt = new TimedEventCollection.Event("test", TimeSpan.Zero);

        // Allow a tiny window for the event to expire
        evt.Completed
           .Should()
           .BeTrue();
    }
    #endregion
}
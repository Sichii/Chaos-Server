#region
using Chaos.Collections;
using Chaos.Time.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class UpdatableCollectionTests
{
    private readonly UpdatableCollection Collection;

    public UpdatableCollectionTests()
    {
        var loggerMock = new Mock<ILogger>();
        Collection = new UpdatableCollection(loggerMock.Object);
    }

    #region Add
    [Test]
    public void Add_WhenNotUpdating_ShouldAddImmediately()
    {
        var item = new MockUpdatable();

        Collection.Add(item);

        // Verify the item is in the collection by calling Update
        Collection.Update(TimeSpan.FromSeconds(1));

        item.UpdateCount
            .Should()
            .Be(1);
    }
    #endregion

    #region Clear
    [Test]
    public void Clear_WhenNotUpdating_ShouldClearImmediately()
    {
        var item1 = new MockUpdatable();
        var item2 = new MockUpdatable();
        Collection.Add(item1);
        Collection.Add(item2);

        Collection.Clear();

        Collection.Update(TimeSpan.FromSeconds(1));

        item1.UpdateCount
             .Should()
             .Be(0);

        item2.UpdateCount
             .Should()
             .Be(0);
    }
    #endregion

    #region Remove
    [Test]
    public void Remove_WhenNotUpdating_ShouldRemoveImmediately()
    {
        var item = new MockUpdatable();
        Collection.Add(item);

        Collection.Remove(item);

        // Item was removed, so Update should not affect it
        Collection.Update(TimeSpan.FromSeconds(1));

        item.UpdateCount
            .Should()
            .Be(0);
    }
    #endregion

    private sealed class MockUpdatable : IDeltaUpdatable
    {
        public Action<TimeSpan>? OnUpdate { get; set; }
        public int UpdateCount { get; private set; }

        public void Update(TimeSpan delta)
        {
            UpdateCount++;
            OnUpdate?.Invoke(delta);
        }
    }

    private sealed class ThrowingUpdatable : IDeltaUpdatable
    {
        public void Update(TimeSpan delta) => throw new InvalidOperationException("Test error");
    }

    #region Update
    [Test]
    public void Update_ShouldUpdateAllItems()
    {
        var item1 = new MockUpdatable();
        var item2 = new MockUpdatable();
        Collection.Add(item1);
        Collection.Add(item2);

        Collection.Update(TimeSpan.FromSeconds(1));

        item1.UpdateCount
             .Should()
             .Be(1);

        item2.UpdateCount
             .Should()
             .Be(1);
    }

    [Test]
    public void Update_ShouldContinueAfterException()
    {
        var thrower = new ThrowingUpdatable();
        var normalItem = new MockUpdatable();
        Collection.Add(thrower);
        Collection.Add(normalItem);

        // Should not throw; the exception is caught internally
        var act = () => Collection.Update(TimeSpan.FromSeconds(1));

        act.Should()
           .NotThrow();

        // The normal item should still have been updated
        normalItem.UpdateCount
                  .Should()
                  .Be(1);
    }

    [Test]
    public void Update_ShouldProcessDeferredAdd()
    {
        var deferredItem = new MockUpdatable();

        // Add an item that will trigger a deferred add during update
        var trigger = new MockUpdatable
        {
            OnUpdate = _ => Collection.Add(deferredItem)
        };

        Collection.Add(trigger);

        // First update: trigger adds deferredItem; deferred item is NOT updated yet
        Collection.Update(TimeSpan.FromSeconds(1));

        deferredItem.UpdateCount
                    .Should()
                    .Be(0);

        // Second update: deferredItem is now in the collection
        Collection.Update(TimeSpan.FromSeconds(1));

        deferredItem.UpdateCount
                    .Should()
                    .Be(1);
    }

    [Test]
    public void Update_ShouldProcessDeferredRemove()
    {
        var itemToRemove = new MockUpdatable();
        Collection.Add(itemToRemove);

        // Add a trigger that removes itemToRemove during update
        var trigger = new MockUpdatable
        {
            OnUpdate = _ => Collection.Remove(itemToRemove)
        };

        Collection.Add(trigger);

        // First update: both are updated, but removal is deferred
        Collection.Update(TimeSpan.FromSeconds(1));

        itemToRemove.UpdateCount
                    .Should()
                    .Be(1);

        // Second update: itemToRemove has been removed
        Collection.Update(TimeSpan.FromSeconds(1));

        itemToRemove.UpdateCount
                    .Should()
                    .Be(1); // Still 1, not updated again
    }

    [Test]
    public void Update_ShouldProcessDeferredClear()
    {
        var item1 = new MockUpdatable();
        var item2 = new MockUpdatable();
        Collection.Add(item1);
        Collection.Add(item2);

        // Add a trigger that clears during update
        var trigger = new MockUpdatable
        {
            OnUpdate = _ => Collection.Clear()
        };

        Collection.Add(trigger);

        // First update: all items updated, but clear is deferred
        Collection.Update(TimeSpan.FromSeconds(1));

        item1.UpdateCount
             .Should()
             .Be(1);

        item2.UpdateCount
             .Should()
             .Be(1);

        // Second update: collection was cleared, no items to update
        Collection.Update(TimeSpan.FromSeconds(1));

        item1.UpdateCount
             .Should()
             .Be(1);

        item2.UpdateCount
             .Should()
             .Be(1);
    }

    [Test]
    public void Update_ShouldProcessMultipleDeferredActionsInOrder()
    {
        var itemA = new MockUpdatable();
        var itemB = new MockUpdatable();

        // Trigger adds itemA, then itemB during the update
        var trigger = new MockUpdatable
        {
            OnUpdate = _ =>
            {
                Collection.Add(itemA);
                Collection.Add(itemB);
            }
        };

        Collection.Add(trigger);

        Collection.Update(TimeSpan.FromSeconds(1));

        // Both should not have been updated yet (deferred)
        itemA.UpdateCount
             .Should()
             .Be(0);

        itemB.UpdateCount
             .Should()
             .Be(0);

        // Now they should both be in the collection
        Collection.Update(TimeSpan.FromSeconds(1));

        itemA.UpdateCount
             .Should()
             .Be(1);

        itemB.UpdateCount
             .Should()
             .Be(1);
    }

    [Test]
    public void Update_DeferredAddThenRemove_ShouldResultInNoItem()
    {
        var item = new MockUpdatable();

        // Trigger adds then removes the same item during update
        var trigger = new MockUpdatable
        {
            OnUpdate = _ =>
            {
                Collection.Add(item);
                Collection.Remove(item);
            }
        };

        Collection.Add(trigger);

        Collection.Update(TimeSpan.FromSeconds(1));

        // Item was added then removed, so it should not be in collection
        Collection.Update(TimeSpan.FromSeconds(1));

        item.UpdateCount
            .Should()
            .Be(0);
    }
    #endregion
}
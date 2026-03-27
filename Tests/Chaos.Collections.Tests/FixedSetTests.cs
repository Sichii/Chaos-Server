#region
using System.Collections;
using Chaos.Collections.Specialized;
using FluentAssertions;
#endregion

namespace Chaos.Collections.Tests;

public sealed class FixedSetTests
{
    [Test]
    public void Add_ExistingItem_ShouldMoveToEnd()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(3)
        {
            "item1",
            "item2",
            "item3",

            // Act - Re-add first item, should move to end
            "item1"
        };

        // Assert
        fixedSet.Count
                .Should()
                .Be(3);
        var items = fixedSet.ToList();

        items[2]
            .Should()
            .Be("item1"); // Should be at the end now
    }

    [Test]
    public void Add_MultipleItemsAtCapacity_ShouldMaintainFIFOOrder()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(3)
        {
            "item1",
            "item2",
            "item3",

            // Act
            "item4",
            "item5"
        };

        // Assert
        fixedSet.Count
                .Should()
                .Be(3);

        fixedSet.Contains("item1")
                .Should()
                .BeFalse();

        fixedSet.Contains("item2")
                .Should()
                .BeFalse();

        fixedSet.Contains("item3")
                .Should()
                .BeTrue();

        fixedSet.Contains("item4")
                .Should()
                .BeTrue();

        fixedSet.Contains("item5")
                .Should()
                .BeTrue();
    }

    [Test]
    public void Add_NewItem_ShouldAddItem()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(3)
        {
            // Act
            "item1"
        };

        // Assert
        fixedSet.Count
                .Should()
                .Be(1);

        fixedSet.Contains("item1")
                .Should()
                .BeTrue();
    }

    [Test]
    public void Add_WhenAtCapacity_ShouldRemoveOldestItem()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(2)
        {
            "item1",
            "item2",

            // Act - Add third item when capacity is 2
            "item3"
        };

        // Assert
        fixedSet.Count
                .Should()
                .Be(2);

        fixedSet.Contains("item1")
                .Should()
                .BeFalse(); // Should be removed

        fixedSet.Contains("item2")
                .Should()
                .BeTrue();

        fixedSet.Contains("item3")
                .Should()
                .BeTrue();
    }

    [Test]
    public void Capacity_ShouldReturnFixedCapacity()
    {
        // Arrange
        const int EXPECTED_CAPACITY = 42;

        // Act
        var fixedSet = new FixedSet<string>(EXPECTED_CAPACITY);

        // Assert
        fixedSet.Capacity
                .Should()
                .Be(EXPECTED_CAPACITY);
    }

    [Test]
    public void Clear_ShouldRemoveAllItems()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(3)
        {
            "item1",
            "item2"
        };

        // Act
        fixedSet.Clear();

        // Assert
        fixedSet.Count
                .Should()
                .Be(0);

        fixedSet.Contains("item1")
                .Should()
                .BeFalse();

        fixedSet.Contains("item2")
                .Should()
                .BeFalse();
    }

    [Test]
    public void Constructor_WithCustomComparer_ShouldUseComparer()
    {
        // Arrange
        var comparer = StringComparer.OrdinalIgnoreCase;

        var items = new[]
        {
            "Item1",
            "ITEM1",
            "item2"
        };

        // Act
        var fixedSet = new FixedSet<string>(5, items, comparer);

        // Assert
        fixedSet.Count
                .Should()
                .Be(2); // "Item1" and "ITEM1" should be treated as duplicates

        fixedSet.Contains("item1")
                .Should()
                .BeTrue(); // Case-insensitive lookup

        fixedSet.Contains("ITEM2")
                .Should()
                .BeTrue(); // Case-insensitive lookup
    }

    [Test]
    public void Constructor_WithDuplicateItems_ShouldFilterDuplicates()
    {
        // Arrange
        var items = new[]
        {
            "item1",
            "item2",
            "item1",
            "item3",
            "item2"
        };

        // Act
        var fixedSet = new FixedSet<string>(5, items);

        // Assert
        fixedSet.Count
                .Should()
                .Be(3);

        fixedSet.Contains("item1")
                .Should()
                .BeTrue();

        fixedSet.Contains("item2")
                .Should()
                .BeTrue();

        fixedSet.Contains("item3")
                .Should()
                .BeTrue();
    }

    [Test]
    public void Constructor_WithItems_ShouldPopulateSet()
    {
        // Arrange
        var items = new[]
        {
            "item1",
            "item2",
            "item3"
        };

        // Act
        var fixedSet = new FixedSet<string>(5, items);

        // Assert
        fixedSet.Count
                .Should()
                .Be(3);

        fixedSet.Contains("item1")
                .Should()
                .BeTrue();

        fixedSet.Contains("item2")
                .Should()
                .BeTrue();

        fixedSet.Contains("item3")
                .Should()
                .BeTrue();
    }

    [Test]
    public void Constructor_WithNegativeCapacity_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange & Act
        var act = () => new FixedSet<string>(-1);

        // Assert
        act.Should()
           .Throw<ArgumentOutOfRangeException>()
           .WithParameterName("capacity")
           .WithMessage("*Capacity must be greater than zero.*");
    }

    [Test]
    public void Constructor_WithNullItems_ShouldCreateEmptySet()
    {
        // Arrange & Act
        // ReSharper disable once CollectionNeverUpdated.Local
        var fixedSet = new FixedSet<string>(3);

        // Assert
        fixedSet.Count
                .Should()
                .Be(0);
    }

    [Test]
    public void Constructor_WithValidCapacity_ShouldCreateEmptySet()
    {
        // Arrange & Act
        var fixedSet = new FixedSet<string>(3);

        // Assert
        fixedSet.Capacity
                .Should()
                .Be(3);

        fixedSet.Count
                .Should()
                .Be(0);

        fixedSet.IsReadOnly
                .Should()
                .BeFalse();
    }

    [Test]
    public void Constructor_WithZeroCapacity_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange & Act
        var act = () => new FixedSet<string>(0);

        // Assert
        act.Should()
           .Throw<ArgumentOutOfRangeException>()
           .WithParameterName("capacity")
           .WithMessage("*Capacity must be greater than zero.*");
    }

    [Test]
    public void Contains_ExistingItem_ShouldReturnTrue()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(3)
        {
            "item1"
        };

        // Act & Assert
        fixedSet.Contains("item1")
                .Should()
                .BeTrue();
    }

    [Test]
    public void Contains_NonExistingItem_ShouldReturnFalse()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(3)
        {
            "item1"
        };

        // Act & Assert
        fixedSet.Contains("item2")
                .Should()
                .BeFalse();
    }

    [Test]
    public void CopyTo_ShouldCopyItemsToArray()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(5)
        {
            "item1",
            "item2",
            "item3"
        };
        var array = new string[5];

        // Act
        fixedSet.CopyTo(array, 1);

        // Assert
        array[0]
            .Should()
            .BeNull();

        array[1]
            .Should()
            .Be("item1");

        array[2]
            .Should()
            .Be("item2");

        array[3]
            .Should()
            .Be("item3");

        array[4]
            .Should()
            .BeNull();
    }

    [Test]
    public void Count_ShouldReflectActualItemCount()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(5);

        // Act & Assert
        fixedSet.Count
                .Should()
                .Be(0);

        fixedSet.Add("item1");

        fixedSet.Count
                .Should()
                .Be(1);

        fixedSet.Add("item2");

        fixedSet.Count
                .Should()
                .Be(2);

        fixedSet.Remove("item1");

        fixedSet.Count
                .Should()
                .Be(1);

        fixedSet.Clear();

        fixedSet.Count
                .Should()
                .Be(0);
    }

    [Test]
    public void EdgeCase_ReAddingSameItemMultipleTimes_ShouldKeepOnlyOne()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(3)
        {
            // Act
            "item",
            "item",
            "item"
        };

        // Assert
        fixedSet.Count
                .Should()
                .Be(1);

        fixedSet.Contains("item")
                .Should()
                .BeTrue();
    }

    [Test]
    public void EdgeCase_SingleItemCapacity_ShouldWork()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(1)
        {
            // Act
            "first",
            "second" // Should replace first
        };

        // Assert
        fixedSet.Count
                .Should()
                .Be(1);

        fixedSet.Contains("first")
                .Should()
                .BeFalse();

        fixedSet.Contains("second")
                .Should()
                .BeTrue();
    }

    [Test]
    public void GetEnumerator_Generic_ShouldReturnItemsInOldestToNewestOrder()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(3)
        {
            "first",
            "second",
            "third"
        };

        // Act
        var items = fixedSet.ToList();

        // Assert
        items.Should()
             .ContainInOrder("first", "second", "third");
    }

    [Test]
    public void GetEnumerator_NonGeneric_ShouldReturnItemsInOldestToNewestOrder()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(3)
        {
            "first",
            "second",
            "third"
        };

        // Act
        var items = new List<string>();
        var enumerator = ((IEnumerable)fixedSet).GetEnumerator();
        using var enumerator1 = enumerator as IDisposable;

        while (enumerator.MoveNext())
            items.Add((string)enumerator.Current!);

        // Assert
        items.Should()
             .ContainInOrder("first", "second", "third");
    }

    [Test]
    public void GetEnumerator_ShouldReturnSnapshot_ConcurrentModificationsShouldNotAffectEnumeration()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(3)
        {
            "item1",
            "item2"
        };

        // Act
        using var enumerator = fixedSet.GetEnumerator();
        fixedSet.Add("item3"); // Modify during enumeration
        var items = new List<string>();

        while (enumerator.MoveNext())
            items.Add(enumerator.Current);

        // Assert
        items.Should()
             .HaveCount(2); // Snapshot should have original 2 items

        items.Should()
             .Contain("item1");

        items.Should()
             .Contain("item2");

        items.Should()
             .NotContain("item3");
    }

    [Test]
    public void IsReadOnly_ShouldAlwaysReturnFalse()
    {
        // Arrange & Act
        // ReSharper disable once CollectionNeverUpdated.Local
        var fixedSet = new FixedSet<string>(3);

        // Assert
        fixedSet.IsReadOnly
                .Should()
                .BeFalse();
    }

    [Test]
    public void Remove_ExistingItem_ShouldReturnTrueAndRemoveItem()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(3)
        {
            "item1",
            "item2"
        };

        // Act
        var result = fixedSet.Remove("item1");

        // Assert
        result.Should()
              .BeTrue();

        fixedSet.Count
                .Should()
                .Be(1);

        fixedSet.Contains("item1")
                .Should()
                .BeFalse();

        fixedSet.Contains("item2")
                .Should()
                .BeTrue();
    }

    [Test]
    public void Remove_NonExistingItem_ShouldReturnFalse()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(3)
        {
            "item1"
        };

        // Act
        var result = fixedSet.Remove("item2");

        // Assert
        result.Should()
              .BeFalse();

        fixedSet.Count
                .Should()
                .Be(1);
    }

    [Test]
    public void ThreadSafety_ConcurrentAddOperations_ShouldNotCorruptState()
    {
        // Arrange
        var fixedSet = new FixedSet<int>(100);
        var tasks = new List<Task>();
        const int NUMBER_OF_TASKS = 10;
        const int ITEMS_PER_TASK = 50;

        // Act - Multiple threads adding items concurrently
        for (var i = 0; i < NUMBER_OF_TASKS; i++)
        {
            var taskId = i;

            tasks.Add(
                Task.Run(() =>
                {
                    for (var j = 0; j < ITEMS_PER_TASK; j++)
                        fixedSet.Add(taskId * ITEMS_PER_TASK + j);
                }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        fixedSet.Count
                .Should()
                .BeLessThanOrEqualTo(100); // Should not exceed capacity

        fixedSet.Count
                .Should()
                .BeGreaterThan(0); // Should have some items
    }

    [Test]
    public async Task ThreadSafety_ConcurrentReadWriteOperations_ShouldNotThrow()
    {
        // Arrange
        var fixedSet = new FixedSet<int>(50);
        var cancellationTokenSource = new CancellationTokenSource();
        var tasks = new List<Task>();

        // Pre-populate
        for (var i = 0; i < 25; i++)
            fixedSet.Add(i);

        // Act - Concurrent read/write operations
        tasks.Add(
            Task.Run(() =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                    fixedSet.Add(Random.Shared.Next(1000));
            }));

        tasks.Add(
            Task.Run(() =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)

                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    fixedSet.Contains(Random.Shared.Next(1000));
            }));

        tasks.Add(
            Task.Run(() =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                    fixedSet.Remove(Random.Shared.Next(1000));
            }));

        tasks.Add(
            Task.Run(() =>
            {
                //iteration
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                    foreach (var item in fixedSet)
                        _ = item;
            }));

        // Let it run for a short time
        await Task.Delay(100);
        await cancellationTokenSource.CancelAsync();

        // Assert - Should not throw
        var act = async () => await Task.WhenAll(tasks);

        await act.Should()
                 .NotThrowAsync();
    }
}
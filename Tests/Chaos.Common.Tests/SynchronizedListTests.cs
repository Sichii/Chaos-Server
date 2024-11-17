#region
using Chaos.Collections.Synchronized;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class SynchronizedListTests
{
    [Test]
    public void Add_ShouldAddItemToList()
    {
        // Arrange
        var list = new SynchronizedList<int>
        {
            // Act
            42
        };

        // Assert
        list.Count
            .Should()
            .Be(1);

        list.Should()
            .Contain(42);
    }

    [Test]
    public void Clear_ShouldRemoveAllItemsFromList()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);

        // Act
        list.Clear();

        // Assert
        list.Count
            .Should()
            .Be(0);
    }

    [Test]
    public void Contains_ShouldReturnFalseIfItemDoesNotExistInList()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);

        // Act
        var contains = list.Contains(4);

        // Assert
        contains.Should()
                .BeFalse();
    }

    [Test]
    public void Contains_ShouldReturnTrueIfItemExistsInList()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);

        // Act
        var contains = list.Contains(2);

        // Assert
        contains.Should()
                .BeTrue();
    }

    [Test]
    public void CopyTo_ShouldCopyItemsToArray()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);
        var array = new int[3];

        // Act
        list.CopyTo(array, 0);

        // Assert
        array.Should()
             .BeEquivalentTo(
                 [
                     1,
                     2,
                     3
                 ]);
    }

    [Test]
    public void Count_ShouldReturnCorrectCount()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);

        // Act
        var count = list.Count;

        // Assert
        count.Should()
             .Be(3);
    }

    [Test]
    public void GetEnumerator_ShouldEnumerateItemsInList()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);

        // Act
        var result = list.ToList();

        // Assert
        result.Should()
              .BeEquivalentTo(
                  [
                      1,
                      2,
                      3
                  ]);
    }

    [Test]
    public void Indexer_ShouldGetAndSetItemAtIndex()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);

        // Act
        var item = list[1];
        list[1] = 42;

        // Assert
        item.Should()
            .Be(2);

        list[1]
            .Should()
            .Be(42);
    }

    // Idk why this doesn't throw IndexOutOfRangeException
    [Test]
    public void Indexer_ShouldThrowArgumentOutOfRangeException_WhenIndexIsOutOfRange()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);

        // Act & Assert
        list.Invoking(
                l =>
                {
                    _ = l[5];
                })
            .Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void Indexer_ShouldThrowNotSupportedException_WhenSettingItemAtIndex_WhenListIsReadOnly()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]).AsReadOnly();

        // Act & Assert
        list.Invoking(l => ((IList<int>)l)[1] = 42)
            .Should()
            .Throw<NotSupportedException>();
    }

    [Test]
    public void IndexOf_ShouldReturnIndexForExistingItem()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);

        // Act
        var index = list.IndexOf(2);

        // Assert
        index.Should()
             .Be(1);
    }

    [Test]
    public void IndexOf_ShouldReturnNegativeOneForNonExistingItem()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);

        // Act
        var index = list.IndexOf(4);

        // Assert
        index.Should()
             .Be(-1);
    }

    [Test]
    public void Insert_ShouldInsertItemAtSpecifiedIndex()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);

        // Act
        list.Insert(1, 42);

        // Assert
        list.Count
            .Should()
            .Be(4);

        list[1]
            .Should()
            .Be(42);
    }

    [Test]
    public void IsReadOnly_ShouldReturnFalse()
    {
        // Arrange
        // ReSharper disable once CollectionNeverUpdated.Local
        var list = new SynchronizedList<int>();

        // Act
        var isReadOnly = list.IsReadOnly;

        // Assert
        isReadOnly.Should()
                  .BeFalse();
    }

    [Test]
    public void Remove_ShouldRemoveItemFromList()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);

        // Act
        var removed = list.Remove(2);

        // Assert
        list.Count
            .Should()
            .Be(2);

        removed.Should()
               .BeTrue();

        list.Should()
            .NotContain(2);
    }

    [Test]
    public void RemoveAt_ShouldRemoveItemAtSpecifiedIndex()
    {
        // Arrange
        var list = new SynchronizedList<int>(
            [
                1,
                2,
                3
            ]);

        // Act
        list.RemoveAt(1);

        // Assert
        list.Count
            .Should()
            .Be(2);

        list.Should()
            .NotContain(2);
    }
}
using Chaos.Common.Collections.Synchronized;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class SynchronizedListTests
{
    [Fact]
    public void Add_ShouldAddItemToList()
    {
        // Arrange
        var list = new SynchronizedList<int>();

        // Act
        list.Add(42);

        // Assert
        list.Count.Should().Be(1);
        list.Should().Contain(42);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItemsFromList()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });

        // Act
        list.Clear();

        // Assert
        list.Count.Should().Be(0);
    }

    [Fact]
    public void Contains_ShouldReturnFalseIfItemDoesNotExistInList()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });

        // Act
        var contains = list.Contains(4);

        // Assert
        contains.Should().BeFalse();
    }

    [Fact]
    public void Contains_ShouldReturnTrueIfItemExistsInList()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });

        // Act
        var contains = list.Contains(2);

        // Assert
        contains.Should().BeTrue();
    }

    [Fact]
    public void CopyTo_ShouldCopyItemsToArray()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });
        var array = new int[3];

        // Act
        list.CopyTo(array, 0);

        // Assert
        array.Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Fact]
    public void Count_ShouldReturnCorrectCount()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });

        // Act
        var count = list.Count;

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public void GetEnumerator_ShouldEnumerateItemsInList()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });

        // Act
        var result = list.ToList();

        // Assert
        result.Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Fact]
    public void Indexer_ShouldGetAndSetItemAtIndex()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });

        // Act
        var item = list[1];
        list[1] = 42;

        // Assert
        item.Should().Be(2);
        list[1].Should().Be(42);
    }

    // Idk why this doesn't throw IndexOutOfRangeException
    [Fact]
    public void Indexer_ShouldThrowArgumentOutOfRangeException_WhenIndexIsOutOfRange()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });

        // Act & Assert
        list.Invoking(
                l =>
                {
                    _ = l[5];
                })
            .Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Indexer_ShouldThrowNotSupportedException_WhenSettingItemAtIndex_WhenListIsReadOnly()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 }).AsReadOnly();

        // Act & Assert
        list.Invoking(l => ((IList<int>)l)[1] = 42).Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void IndexOf_ShouldReturnIndexForExistingItem()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });

        // Act
        var index = list.IndexOf(2);

        // Assert
        index.Should().Be(1);
    }

    [Fact]
    public void IndexOf_ShouldReturnNegativeOneForNonExistingItem()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });

        // Act
        var index = list.IndexOf(4);

        // Assert
        index.Should().Be(-1);
    }

    [Fact]
    public void Insert_ShouldInsertItemAtSpecifiedIndex()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });

        // Act
        list.Insert(1, 42);

        // Assert
        list.Count.Should().Be(4);
        list[1].Should().Be(42);
    }

    [Fact]
    public void IsReadOnly_ShouldReturnFalse()
    {
        // Arrange
        var list = new SynchronizedList<int>();

        // Act
        var isReadOnly = list.IsReadOnly;

        // Assert
        isReadOnly.Should().BeFalse();
    }

    [Fact]
    public void Remove_ShouldRemoveItemFromList()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });

        // Act
        var removed = list.Remove(2);

        // Assert
        list.Count.Should().Be(2);
        removed.Should().BeTrue();
        list.Should().NotContain(2);
    }

    [Fact]
    public void RemoveAt_ShouldRemoveItemAtSpecifiedIndex()
    {
        // Arrange
        var list = new SynchronizedList<int>(new[] { 1, 2, 3 });

        // Act
        list.RemoveAt(1);

        // Assert
        list.Count.Should().Be(2);
        list.Should().NotContain(2);
    }
}
using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Common.Tests;

public sealed class CollectionExtensionsTests
{
    private readonly IComparer<int> _intComparer = Comparer<int>.Default;

    [Fact]
    public void AddRange_ShouldAddMultipleItemsToCollection()
    {
        // Arrange
        ICollection<int> initialItems = new List<int>
        {
            1,
            2,
            3
        };

        var itemsToAdd = new List<int>
        {
            4,
            5,
            6
        };

        var expectedCollection = new List<int>
        {
            1,
            2,
            3,
            4,
            5,
            6
        };

        // Act
        initialItems.AddRange(itemsToAdd);

        // Assert
        initialItems.Should()
                    .BeEquivalentTo(expectedCollection, "because all items should be added to the collection");
    }

    [Fact]
    public void AddRange_WithNullCollection_ShouldThrowArgumentNullException()
    {
        // Arrange
        ICollection<int>? nullCollection = null;

        var itemsToAdd = new List<int>
        {
            1,
            2,
            3
        };

        // Act
        var act = () => nullCollection!.AddRange(itemsToAdd);

        // Assert
        act.Should()
           .Throw<ArgumentNullException>()
           .WithMessage("*collection*", "because a null collection should cause an ArgumentNullException");
    }

    [Fact]
    public void BinarySearch_EmptyCollection_ReturnsComplementOfZero()
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        IList<int> collection = new List<int>();

        var index = collection.BinarySearch(5, _intComparer);

        index.Should()
             .Be(~0, "because the collection is empty");
    }

    [Fact]
    public void BinarySearch_ItemLargerThanAllElements_ReturnsComplementOfCollectionCount()
    {
        IList<int> collection = new List<int>
        {
            1,
            3,
            5,
            7,
            9
        };

        var index = collection.BinarySearch(11, _intComparer);

        index.Should()
             .Be(~5, "because 11 is larger than all elements in the collection");
    }

    [Fact]
    public void BinarySearch_ItemNotPresentButHasGreaterElements_ReturnsComplementOfNextLargerItemIndex()
    {
        IList<int> collection = new List<int>
        {
            1,
            3,
            5,
            7,
            9
        };

        var index = collection.BinarySearch(6, _intComparer);

        index.Should()
             .Be(~3, "because 6 is not present and 7 (at index 3) is the next larger item");
    }

    [Fact]
    public void BinarySearch_ItemPresent_ReturnsItemIndex()
    {
        IList<int> collection = new List<int>
        {
            1,
            3,
            5,
            7,
            9
        };

        var index = collection.BinarySearch(5, _intComparer);

        index.Should()
             .Be(2, "because the item 5 is at index 2");
    }

    [Fact]
    public void BinarySearch_SingleItemCollection_ItemNotPresent_ReturnsComplementOfCollectionCount()
    {
        IList<int> collection = new List<int>
        {
            5
        };

        var index = collection.BinarySearch(7, _intComparer);

        index.Should()
             .Be(~1, "because the item is not present and it's larger than the single item in the collection");
    }

    [Fact]
    public void BinarySearch_SingleItemCollection_ItemPresent_ReturnsZero()
    {
        IList<int> collection = new List<int>
        {
            5
        };

        var index = collection.BinarySearch(5, _intComparer);

        index.Should()
             .Be(0, "because there's a single item and it matches the search");
    }

    [Fact]
    public void Replace_EmptyCollection_DoesNotModifyCollectionAndReturnsFalse()
    {
        var collection = new List<string>();
        var result = collection.Replace("apple", "mango");

        result.Should()
              .BeFalse("because the collection is empty");

        collection.Should()
                  .BeEmpty("because the collection should remain empty");
    }

    [Fact]
    public void Replace_ItemNotPresent_DoesNotModifyCollectionAndReturnsFalse()
    {
        var collection = new List<string>
        {
            "apple",
            "banana",
            "cherry"
        };
        var originalList = new List<string>(collection); // copy for comparison

        var result = collection.Replace("grape", "mango");

        result.Should()
              .BeFalse("because the item 'grape' is not present");

        collection.Should()
                  .Equal(originalList, "because the collection should not be modified");
    }

    [Fact]
    public void Replace_ItemPresent_ReplacesItemAndReturnsTrue()
    {
        var collection = new List<string>
        {
            "apple",
            "banana",
            "cherry"
        };

        var result = collection.Replace("banana", "mango");

        result.Should()
              .BeTrue("because the item 'banana' is present and should be replaced");

        collection[1]
            .Should()
            .Be("mango", "because 'banana' should be replaced with 'mango'");
    }

    [Fact]
    public void Replace_MultipleInstancesOfItem_OnlyFirstIsReplacedAndReturnsTrue()
    {
        var collection = new List<string>
        {
            "apple",
            "banana",
            "cherry",
            "banana"
        };

        var result = collection.Replace("banana", "mango");

        result.Should()
              .BeTrue("because the item 'banana' is present and should be replaced");

        collection[1]
            .Should()
            .Be("mango", "because the first instance of 'banana' should be replaced with 'mango'");

        collection[3]
            .Should()
            .Be("banana", "because the second instance of 'banana' should remain unchanged");
    }

    [Fact]
    public void ReplaceBy_EmptyCollection_DoesNotModifyCollectionAndReturnsFalse()
    {
        var collection = new List<int>();
        var result = collection.ReplaceBy(x => x == 1, 8);

        result.Should()
              .BeFalse("because the collection is empty");

        collection.Should()
                  .BeEmpty("because the collection should remain empty");
    }

    [Fact]
    public void ReplaceBy_ItemMatchingPredicate_ReplacesItemAndReturnsTrue()
    {
        var collection = new List<int>
        {
            1,
            2,
            3,
            4,
            5
        };

        var result = collection.ReplaceBy(x => x == 3, 8);

        result.Should()
              .BeTrue("because an item matching the predicate is present and should be replaced");

        collection[2]
            .Should()
            .Be(8, "because '3' should be replaced with '8'");
    }

    [Fact]
    public void ReplaceBy_MultipleItemsMatchingPredicate_OnlyFirstIsReplacedAndReturnsTrue()
    {
        var collection = new List<int>
        {
            1,
            2,
            3,
            3,
            5
        };

        var result = collection.ReplaceBy(x => x == 3, 8);

        result.Should()
              .BeTrue("because items matching the predicate are present and should be replaced");

        collection[2]
            .Should()
            .Be(8, "because the first instance of '3' should be replaced with '8'");

        collection[3]
            .Should()
            .Be(3, "because the second instance of '3' should remain unchanged");
    }

    [Fact]
    public void ReplaceBy_NoItemMatchingPredicate_DoesNotModifyCollectionAndReturnsFalse()
    {
        var collection = new List<int>
        {
            1,
            2,
            3,
            4,
            5
        };
        var originalList = new List<int>(collection); // copy for comparison

        var result = collection.ReplaceBy(x => x == 10, 8);

        result.Should()
              .BeFalse("because no item matching the predicate is present");

        collection.Should()
                  .Equal(originalList, "because the collection should not be modified");
    }
}
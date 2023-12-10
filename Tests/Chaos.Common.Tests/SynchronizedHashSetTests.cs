using Chaos.Collections.Synchronized;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class SynchronizedHashSetTests
{
    [Fact]
    public void Add_Should_Add_Item_To_Set()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>
        {
            // Act
            42
        };

        // Assert
        set.Should()
           .Contain(42);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItemsFromSet()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        set.Clear();

        // Assert
        set.Should()
           .BeEmpty();
    }

    [Fact]
    public void Contains_Should_Return_True_When_Item_Exists_In_Set()
    {
        // Arrange
        var set = new SynchronizedHashSet<string>(
            new[]
            {
                "apple",
                "banana",
                "orange"
            });

        // Act
        var contains = set.Contains("banana");

        // Assert
        contains.Should()
                .BeTrue();
    }

    [Fact]
    public void CopyTo_Should_Copy_Items_To_Array_Starting_At_Given_Index()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(
            new[]
            {
                1,
                2,
                3,
                4
            });
        var array = new int[5];

        // Act
        set.CopyTo(array, 1);

        // Assert
        array.Should()
             .Equal(
                 0,
                 1,
                 2,
                 3,
                 4);
    }

    [Fact]
    public void Count_Should_Return_Correct_Count()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        var count = set.Count;

        // Assert
        count.Should()
             .Be(5);
    }

    [Fact]
    public void ExceptWith_ShouldRemoveSpecifiedItemsFromSet()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        set.ExceptWith(
            new[]
            {
                2,
                4
            });

        // Assert
        set.Should()
           .BeEquivalentTo(
               new[]
               {
                   1,
                   3,
                   5
               });
    }

    [Fact]
    public void IntersectWith_Should_Only_Keep_Common_Items_In_Set()
    {
        // Arrange
        var set1 = new SynchronizedHashSet<int>(
            new[]
            {
                1,
                2,
                3,
                4,
                5
            });

        var set2 = new SynchronizedHashSet<int>(
            new[]
            {
                3,
                4,
                5,
                6,
                7
            });

        // Act
        set1.IntersectWith(set2);

        // Assert
        set1.Should()
            .Equal(3, 4, 5);
    }

    [Fact]
    public void IsProperSubsetOf_ShouldReturnTrueWhenSetIsProperSubset()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        var result = set.IsProperSubsetOf(
            new[]
            {
                0,
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                10
            });

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void IsProperSupersetOf_ShouldReturnTrueWhenSetIsProperSuperset()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        var result = set.IsProperSupersetOf(
            new[]
            {
                2,
                4
            });

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void IsSubsetOf_ShouldReturnTrueWhenSetIsSubset()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        var result = set.IsSubsetOf(
            new[]
            {
                0,
                1,
                2,
                3,
                4,
                5,
                6
            });

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void IsSupersetOf_ShouldReturnTrueWhenSetIsSuperset()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        var result = set.IsSupersetOf(
            new[]
            {
                2,
                4
            });

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void Overlaps_ShouldReturnTrueWhenSetOverlapsWithOtherSet()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        var result = set.Overlaps(
            new[]
            {
                4,
                6,
                8
            });

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void Remove_Should_Remove_Item_From_Set()
    {
        // Arrange
        var set = new SynchronizedHashSet<string>(
            new[]
            {
                "apple",
                "banana",
                "orange"
            });

        // Act
        var removed = set.Remove("banana");

        // Assert
        removed.Should()
               .BeTrue();

        set.Should()
           .NotContain("banana");
    }

    [Fact]
    public void SetEquals_Should_Return_True_When_Sets_Are_Equal()
    {
        // Arrange
        var set1 = new SynchronizedHashSet<int>(
            new[]
            {
                1,
                2,
                3
            });

        var set2 = new SynchronizedHashSet<int>(
            new[]
            {
                3,
                2,
                1
            });

        // Act
        var equals = set1.SetEquals(set2);

        // Assert
        equals.Should()
              .BeTrue();
    }

    [Fact]
    public void SymmetricExceptWith_ShouldPerformSymmetricDifferenceOperation()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        set.SymmetricExceptWith(
            new[]
            {
                2,
                4,
                6
            });

        // Assert
        set.Should()
           .BeEquivalentTo(
               new[]
               {
                   1,
                   3,
                   5,
                   6
               });
    }

    [Fact]
    public void TryGetValue_ShouldRetrieveValueForExistingKey()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>
        {
            42
        };

        // Act
        var result = set.TryGetValue(42, out var actualValue);

        // Assert
        result.Should()
              .BeTrue();

        actualValue.Should()
                   .Be(42);
    }

    [Fact]
    public void TryGetValue_ShouldReturnFalseForNonExistingKey()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>
        {
            42
        };

        // Act
        var result = set.TryGetValue(0, out var actualValue);

        // Assert
        result.Should()
              .BeFalse();

        actualValue.Should()
                   .Be(default);
    }

    [Fact]
    public void UnionWith_Should_Add_Items_From_Other_Set_To_Current_Set()
    {
        // Arrange
        var set1 = new SynchronizedHashSet<int>(
            new[]
            {
                1,
                2,
                3
            });

        var set2 = new SynchronizedHashSet<int>(
            new[]
            {
                3,
                4,
                5
            });

        // Act
        set1.UnionWith(set2);

        // Assert
        set1.Should()
            .Equal(
                1,
                2,
                3,
                4,
                5);
    }
}
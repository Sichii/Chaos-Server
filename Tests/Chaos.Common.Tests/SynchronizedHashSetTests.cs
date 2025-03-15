#region
using Chaos.Collections.Synchronized;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class SynchronizedHashSetTests
{
    [Test]
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

    [Test]
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

    [Test]
    public void Contains_Should_Return_True_When_Item_Exists_In_Set()
    {
        // Arrange
        var set = new SynchronizedHashSet<string>(
            [
                "apple",
                "banana",
                "orange"
            ]);

        // Act
        var contains = set.Contains("banana");

        // Assert
        contains.Should()
                .BeTrue();
    }

    [Test]
    public void CopyTo_Should_Copy_Items_To_Array_Starting_At_Given_Index()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(
            [
                1,
                2,
                3,
                4
            ]);
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

    [Test]
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

    [Test]
    public void ExceptWith_ShouldRemoveSpecifiedItemsFromSet()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        set.ExceptWith(
            [
                2,
                4
            ]);

        // Assert
        set.Should()
           .BeEquivalentTo(
               [
                   1,
                   3,
                   5
               ]);
    }

    [Test]
    public void IntersectWith_Should_Only_Keep_Common_Items_In_Set()
    {
        // Arrange
        var set1 = new SynchronizedHashSet<int>(
            [
                1,
                2,
                3,
                4,
                5
            ]);

        var set2 = new SynchronizedHashSet<int>(
            [
                3,
                4,
                5,
                6,
                7
            ]);

        // Act
        set1.IntersectWith(set2);

        // Assert
        set1.Should()
            .Equal(3, 4, 5);
    }

    [Test]
    public void IsProperSubsetOf_ShouldReturnTrueWhenSetIsProperSubset()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        var result = set.IsProperSubsetOf(
            [
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
            ]);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void IsProperSupersetOf_ShouldReturnTrueWhenSetIsProperSuperset()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        var result = set.IsProperSupersetOf(
            [
                2,
                4
            ]);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void IsSubsetOf_ShouldReturnTrueWhenSetIsSubset()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        var result = set.IsSubsetOf(
            [
                0,
                1,
                2,
                3,
                4,
                5,
                6
            ]);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void IsSupersetOf_ShouldReturnTrueWhenSetIsSuperset()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        var result = set.IsSupersetOf(
            [
                2,
                4
            ]);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Overlaps_ShouldReturnTrueWhenSetOverlapsWithOtherSet()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        var result = set.Overlaps(
            [
                4,
                6,
                8
            ]);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Remove_Should_Remove_Item_From_Set()
    {
        // Arrange
        var set = new SynchronizedHashSet<string>(
            [
                "apple",
                "banana",
                "orange"
            ]);

        // Act
        var removed = set.Remove("banana");

        // Assert
        removed.Should()
               .BeTrue();

        set.Should()
           .NotContain("banana");
    }

    [Test]
    public void SetEquals_Should_Return_True_When_Sets_Are_Equal()
    {
        // Arrange
        var set1 = new SynchronizedHashSet<int>(
            [
                1,
                2,
                3
            ]);

        var set2 = new SynchronizedHashSet<int>(
            [
                3,
                2,
                1
            ]);

        // Act
        var equals = set1.SetEquals(set2);

        // Assert
        equals.Should()
              .BeTrue();
    }

    [Test]
    public void SymmetricExceptWith_ShouldPerformSymmetricDifferenceOperation()
    {
        // Arrange
        var set = new SynchronizedHashSet<int>(Enumerable.Range(1, 5));

        // Act
        set.SymmetricExceptWith(
            [
                2,
                4,
                6
            ]);

        // Assert
        set.Should()
           .BeEquivalentTo(
               [
                   1,
                   3,
                   5,
                   6
               ]);
    }

    [Test]
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

    [Test]
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
                   .Be(0);
    }

    [Test]
    public void UnionWith_Should_Add_Items_From_Other_Set_To_Current_Set()
    {
        // Arrange
        var set1 = new SynchronizedHashSet<int>(
            [
                1,
                2,
                3
            ]);

        var set2 = new SynchronizedHashSet<int>(
            [
                3,
                4,
                5
            ]);

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
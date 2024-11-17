#region
using Chaos.Collections.Common;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class CounterCollectionTests
{
    [Test]
    public void AddOrIncrement_ShouldAddNewCounterWithInitialValue_WhenKeyDoesNotExist()
    {
        // Arrange
        var counterCollection = new CounterCollection();

        // Act
        var value = counterCollection.AddOrIncrement("key");

        // Assert
        value.Should()
             .Be(1);
    }

    [Test]
    public void AddOrIncrement_ShouldIncrementExistingCounterByOne_WhenKeyExists()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.Set("key", 5);

        // Act
        var value = counterCollection.AddOrIncrement("key");

        // Assert
        value.Should()
             .Be(6);
    }

    [Test]
    public void Constructor_ShouldInitializeCountersFromEnumerable()
    {
        // Arrange
        var counters = new List<KeyValuePair<string, int>>
        {
            new("key1", 10),
            new("key2", 20),
            new("key3", 30)
        };

        // Act
        var counterCollection = new CounterCollection(counters);

        // Assert
        counterCollection.ContainsKey("key1")
                         .Should()
                         .BeTrue();

        counterCollection.ContainsKey("key2")
                         .Should()
                         .BeTrue();

        counterCollection.ContainsKey("key3")
                         .Should()
                         .BeTrue();

        counterCollection.TryGetValue("key1", out var value1)
                         .Should()
                         .BeTrue();

        counterCollection.TryGetValue("key2", out var value2)
                         .Should()
                         .BeTrue();

        counterCollection.TryGetValue("key3", out var value3)
                         .Should()
                         .BeTrue();

        value1.Should()
              .Be(10);

        value2.Should()
              .Be(20);

        value3.Should()
              .Be(30);
    }

    [Test]
    public void ContainsKey_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var counterCollection = new CounterCollection();

        // Act
        var containsKey = counterCollection.ContainsKey("key");

        // Assert
        containsKey.Should()
                   .BeFalse();
    }

    [Test]
    public void ContainsKey_ShouldReturnTrue_WhenKeyExists()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.Set("key", 10);

        // Act
        var containsKey = counterCollection.ContainsKey("key");

        // Assert
        containsKey.Should()
                   .BeTrue();
    }

    [Test]
    public void CounterGreaterThanOrEqualTo_ShouldReturnFalse_WhenCounterValueIsLessThanValue()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.Set("key", 3);

        // Act
        var isGreaterThanOrEqualTo = counterCollection.CounterGreaterThanOrEqualTo("key", 5);

        // Assert
        isGreaterThanOrEqualTo.Should()
                              .BeFalse();
    }

    [Test]
    public void CounterGreaterThanOrEqualTo_ShouldReturnTrue_WhenCounterValueIsGreaterThanOrEqualToValue()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.Set("key", 10);

        // Act
        var isGreaterThanOrEqualTo = counterCollection.CounterGreaterThanOrEqualTo("key", 5);

        // Assert
        isGreaterThanOrEqualTo.Should()
                              .BeTrue();
    }

    [Test]
    public void CounterLessThanOrEqualTo_ShouldReturnFalse_WhenCounterValueIsGreaterThanValue()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.Set("key", 7);

        // Act
        var isLessThanOrEqualTo = counterCollection.CounterLessThanOrEqualTo("key", 5);

        // Assert
        isLessThanOrEqualTo.Should()
                           .BeFalse();
    }

    [Test]
    public void CounterLessThanOrEqualTo_ShouldReturnTrue_WhenCounterValueIsLessThanOrEqualToValue()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.Set("key", 5);

        // Act
        var isLessThanOrEqualTo = counterCollection.CounterLessThanOrEqualTo("key", 10);

        // Assert
        isLessThanOrEqualTo.Should()
                           .BeTrue();
    }

    [Test]
    public void GetEnumerator_ShouldEnumerateAllCounters()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.AddOrIncrement("key1");
        counterCollection.AddOrIncrement("key2", 3);
        counterCollection.AddOrIncrement("key3", 5);

        // Act
        var counterList = counterCollection.ToList();

        // Assert
        counterList.Should()
                   .HaveCount(3);

        counterList.Should()
                   .Contain(new KeyValuePair<string, int>("key1", 1));

        counterList.Should()
                   .Contain(new KeyValuePair<string, int>("key2", 3));

        counterList.Should()
                   .Contain(new KeyValuePair<string, int>("key3", 5));
    }

    [Test]
    public void Remove_ShouldRemoveCounterAndReturnItsValue_WhenKeyExists()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.Set("key", 10);

        // Act
        counterCollection.Remove("key", out var value);

        // Assert
        value.Should()
             .Be(10);

        counterCollection.ContainsKey("key")
                         .Should()
                         .BeFalse();
    }

    [Test]
    public void Set_ShouldAddNewCounterWithSpecifiedValue_WhenKeyDoesNotExist()
    {
        // Arrange
        var counterCollection = new CounterCollection();

        // Act
        counterCollection.Set("key", 5);

        // Assert
        counterCollection.TryGetValue("key", out var value)
                         .Should()
                         .BeTrue();

        value.Should()
             .Be(5);
    }

    [Test]
    public void Set_ShouldSetCounterValue_WhenKeyExists()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.Set("key", 5);

        // Act
        counterCollection.Set("key", 10);

        // Assert
        counterCollection.TryGetValue("key", out var value)
                         .Should()
                         .BeTrue();

        value.Should()
             .Be(10);
    }

    [Test]
    public void TryDecrement_ShouldDecrementCounterValueByOneAndReturnTrue_WhenKeyExistsAndValueIsPositive()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.Set("key", 5);

        // Act
        var isSuccessful = counterCollection.TryDecrement("key", out var newValue);

        // Assert
        isSuccessful.Should()
                    .BeTrue();

        newValue.Should()
                .Be(4);
    }

    [Test]
    public void TryDecrement_ShouldReturnFalse_WhenDecrementWouldResultInNegativeValue()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.Set("key", 0);

        // Act
        var isSuccessful = counterCollection.TryDecrement("key", out _);

        // Assert
        isSuccessful.Should()
                    .BeFalse();
    }

    [Test]
    public void TryDecrement_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var counterCollection = new CounterCollection();

        // Act
        var isSuccessful = counterCollection.TryDecrement("key", out _);

        // Assert
        isSuccessful.Should()
                    .BeFalse();
    }

    [Test]
    public void TryDecrementWithValue_ShouldDecrementCounterValueBySpecifiedValueAndReturnTrue_WhenKeyExistsAndValueIsPositive()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.Set("key", 10);

        // Act
        var isSuccessful = counterCollection.TryDecrement("key", 3, out var newValue);

        // Assert
        isSuccessful.Should()
                    .BeTrue();

        newValue.Should()
                .Be(7);
    }

    [Test]
    public void TryDecrementWithValue_ShouldReturnFalse_WhenDecrementWouldResultInNegativeValue()
    {
        // Arrange
        var counterCollection = new CounterCollection();
        counterCollection.Set("key", 2);

        // Act
        var isSuccessful = counterCollection.TryDecrement("key", 5, out _);

        // Assert
        isSuccessful.Should()
                    .BeFalse();
    }

    [Test]
    public void TryDecrementWithValue_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var counterCollection = new CounterCollection();

        // Act
        var isSuccessful = counterCollection.TryDecrement("key", 5, out _);

        // Assert
        isSuccessful.Should()
                    .BeFalse();
    }
}
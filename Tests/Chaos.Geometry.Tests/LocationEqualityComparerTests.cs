using Chaos.Geometry.EqualityComparers;
using FluentAssertions;
using Xunit;

namespace Chaos.Geometry.Tests;

public sealed class LocationEqualityComparerTests
{
    [Fact]
    public void Equals_ReturnsFalse_WhenLocationsAreNotEqual()
    {
        // Arrange
        var location1 = new Location("Map1", 1, 2);
        var location2 = new Location("Map2", 3, 4);

        // Act
        var result = LocationEqualityComparer.Instance.Equals(location1, location2);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Fact]
    public void Equals_ReturnsTrue_WhenLocationsAreEqual()
    {
        // Arrange
        var location1 = new Location("Map1", 1, 2);
        var location2 = new Location("MAP1", 1, 2);

        // Act
        var result = LocationEqualityComparer.Instance.Equals(location1, location2);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentHashCode_WhenLocationsAreNotEqual()
    {
        // Arrange
        var location1 = new Location("Map1", 1, 2);
        var location2 = new Location("Map2", 3, 4);

        // Act
        var hashCode1 = LocationEqualityComparer.Instance.GetHashCode(location1);
        var hashCode2 = LocationEqualityComparer.Instance.GetHashCode(location2);

        // Assert
        hashCode1.Should()
                 .NotBe(hashCode2);
    }

    [Fact]
    public void GetHashCode_ReturnsSameHashCode_WhenLocationsAreEqual()
    {
        // Arrange
        var location1 = new Location("Map1", 1, 2);
        var location2 = new Location("MAP1", 1, 2);

        // Act
        var hashCode1 = LocationEqualityComparer.Instance.GetHashCode(location1);
        var hashCode2 = LocationEqualityComparer.Instance.GetHashCode(location2);

        // Assert
        hashCode1.Should()
                 .Be(hashCode2);
    }
}
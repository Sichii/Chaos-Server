#region
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using FluentAssertions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Extensions.Geometry.Tests;

public sealed class LocationExtensionsTests
{
    //@formatter:off
    [Test]
    [Arguments(0, 0, Direction.Up, 1, 0, -1)]
    [Arguments(0, 0, Direction.Right, 1, 1, 0)]
    [Arguments(0, 0, Direction.Down, 1, 0, 1)]
    [Arguments(0, 0, Direction.Left, 1, -1, 0)]
    //@formatter:on
    public void DirectionalOffset_ShouldReturnExpectedLocation(
        int startX,
        int startY,
        Direction direction,
        int distance,
        int expectedX,
        int expectedY)
    {
        // Arrange
        var location = new Location(null!, startX, startY);

        // Act
        var offsetPoint = location.DirectionalOffset(direction, distance);
        var offsetLocation = new Location(location.Map, offsetPoint);

        // Assert
        offsetLocation.Map
                      .Should()
                      .BeNull();

        offsetLocation.X
                      .Should()
                      .Be(expectedX);

        offsetLocation.Y
                      .Should()
                      .Be(expectedY);
    }

    [Test]
    public void EnsureSameMap_ShouldNotThrowException_WhenLocationsAreOnSameMap()
    {
        // Arrange
        const string MAP = "abcd";
        var location1 = new Location(MAP, 0, 0);
        var location2 = new Location(MAP, 1, 1);

        // Act
        var act = () => LocationExtensions.EnsureSameMap(location1, location2);

        // Assert
        act.Should()
           .NotThrow<InvalidOperationException>();
    }

    [Test]
    public void EnsureSameMap_ShouldThrowException_WhenLocationsAreOnDifferentMaps()
    {
        // Arrange
        const string MAP1 = "abcd";
        const string MAP2 = "123";
        var location1 = new Location(MAP1, 0, 0);
        var location2 = new Location(MAP2, 1, 1);

        // Act
        var act = () => LocationExtensions.EnsureSameMap(location1, location2);

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("* is not on the same map as *");
    }

    [Test]
    public void OnSameMapAs_ShouldReturnFalse_WhenLocationsAreOnDifferentMaps()
    {
        // Arrange
        const string MAP1 = "abcd";
        const string MAP2 = "123";
        var location1 = new Location(MAP1, 0, 0);
        var location2 = new Location(MAP2, 1, 1);

        // Act
        var result = location1.OnSameMapAs(location2);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void OnSameMapAs_ShouldReturnTrue_WhenLocationsAreOnSameMap()
    {
        // Arrange
        const string MAP = "abcd";
        var location1 = new Location(MAP, 0, 0);
        var location2 = new Location(MAP, 1, 1);

        // Act
        var result = location1.OnSameMapAs(location2);

        // Assert
        result.Should()
              .BeTrue();
    }
}
#region
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using FluentAssertions;
using Chaos.Geometry.Abstractions;
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
    public void EnsureSameMap_ILocation_ValueLocation_DoesNotThrow_When_Same_Map()
    {
        ILocation a = new Location("map", 0, 0);
        var b = new ValueLocation("MAP", 1, 1);

        (var mB, var xB, var yB) = (b.Map, b.X, b.Y);
        var act = () => LocationExtensions.EnsureSameMap(a, new ValueLocation(mB, xB, yB));

        act.Should()
           .NotThrow();
    }

    [Test]
    public void EnsureSameMap_ILocation_ValueLocation_Throws_With_Expected_Message_When_Different_Maps()
    {
        ILocation a = new Location("map1", 0, 0);
        var b = new ValueLocation("map2", 1, 1);
        var expected = $"{ILocation.ToString(a)} is not on the same map as {b.ToString()}";

        (var mB, var xB, var yB) = (b.Map, b.X, b.Y);
        var act = () => LocationExtensions.EnsureSameMap(a, new ValueLocation(mB, xB, yB));

        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage(expected);
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
    public void EnsureSameMap_ValueLocation_ILocation_DoesNotThrow_When_Same_Map()
    {
        var a = new ValueLocation("map", 0, 0);
        ILocation b = new Location("MAP", 1, 1);

        (var mA, var xA, var yA) = (a.Map, a.X, a.Y);
        var act = () => LocationExtensions.EnsureSameMap(new ValueLocation(mA, xA, yA), b);

        act.Should()
           .NotThrow();
    }

    [Test]
    public void EnsureSameMap_ValueLocation_ILocation_Throws_With_Expected_Message_When_Different_Maps()
    {
        var a = new ValueLocation("map1", 0, 0);
        ILocation b = new Location("map2", 1, 1);
        var expected = $"{a.ToString()} is not on the same map as {ILocation.ToString(b)}";

        (var mA, var xA, var yA) = (a.Map, a.X, a.Y);
        var act = () => LocationExtensions.EnsureSameMap(new ValueLocation(mA, xA, yA), b);

        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage(expected);
    }

    [Test]
    public void EnsureSameMap_ValueLocation_ValueLocation_DoesNotThrow_When_Same_Map()
    {
        var a = new ValueLocation("map", 0, 0);
        var b = new ValueLocation("MAP", 1, 1);
        (var mA, var xA, var yA) = (a.Map, a.X, a.Y);
        (var mB, var xB, var yB) = (b.Map, b.X, b.Y);

        var act = () => LocationExtensions.EnsureSameMap(new ValueLocation(mA, xA, yA), new ValueLocation(mB, xB, yB));

        act.Should()
           .NotThrow();
    }

    [Test]
    public void EnsureSameMap_ValueLocation_ValueLocation_Throws_With_Expected_Message_When_Different_Maps()
    {
        var a = new ValueLocation("map1", 0, 0);
        var b = new ValueLocation("map2", 1, 1);

        var expected = $"{a.ToString()} is not on the same map as {b.ToString()}";
        (var mA, var xA, var yA) = (a.Map, a.X, a.Y);
        (var mB, var xB, var yB) = (b.Map, b.X, b.Y);
        var act = () => LocationExtensions.EnsureSameMap(new ValueLocation(mA, xA, yA), new ValueLocation(mB, xB, yB));

        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage(expected);
    }

    [Test]
    public void OnSameMapAs_ILocation_ILocation_IsCaseInsensitive()
    {
        ILocation a = new Location("AbCd", 0, 0);
        ILocation b = new Location("aBcD", 1, 1);

        a.OnSameMapAs(b)
         .Should()
         .BeTrue();
    }

    //formatter:off
    [Test]
    [Arguments("map", "map", true)]
    [Arguments("MAP", "map", true)]
    [Arguments("map1", "map2", false)]

    //formatter:on
    public void OnSameMapAs_ILocation_ValueLocation(string map1, string map2, bool expected)

    {
        ILocation i1 = new Location(map1, 0, 0);
        var v2 = new ValueLocation(map2, 1, 1);

        i1.OnSameMapAs(v2)
          .Should()
          .Be(expected);
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

    //formatter:off
    [Test]
    [Arguments("map", "map", true)]
    [Arguments("map", "MAP", true)]
    [Arguments("map1", "map2", false)]
    public void OnSameMapAs_ValueLocation_ILocation(string map1, string map2, bool expected)

        //formatter:on
    {
        var v1 = new ValueLocation(map1, 0, 0);
        ILocation i2 = new Location(map2, 1, 1);

        v1.OnSameMapAs(i2)
          .Should()
          .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments("map", "map", true)]
    [Arguments("map", "MAP", true)]
    [Arguments("map1", "map2", false)]
    public void OnSameMapAs_ValueLocation_ValueLocation(string map1, string map2, bool expected)

        //formatter:on
    {
        var v1 = new ValueLocation(map1, 0, 0);
        var v2 = new ValueLocation(map2, 1, 1);

        v1.OnSameMapAs(v2)
          .Should()
          .Be(expected);
    }
}
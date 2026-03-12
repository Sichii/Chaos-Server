#region
using Chaos.Geometry.Abstractions;
using FluentAssertions;

// ReSharper disable ArrangeAttributes
#endregion

namespace Chaos.Geometry.Tests;

public sealed class ILocationTests
{
    [Test]
    public void ILocation_ToString_IncludesMapAndCoordinates()
    {
        // Arrange
        const string MAP = "myMap";
        const int X = 10;
        const int Y = 20;
        ILocation location = new Location(MAP, X, Y);

        // Act
        var result = location.ToString();

        // Assert
        result.Should()
              .Contain(MAP)
              .And
              .Contain("10")
              .And
              .Contain("20");
    }

    [Test]
    public void ILocation_ToString_Instance_And_Static_ReturnSameResult_ForLocation()
    {
        // Arrange
        ILocation location = new Location("testMap", 42, 84);

        // Act
        var instanceResult = location.ToString();
        var staticResult = ILocation.ToString(location);

        // Assert
        instanceResult.Should()
                      .Be(staticResult);
    }

    //formatter:off
    [Test]
    [Arguments(
        "map1",
        0,
        0,
        "map1:(0, 0)")]
    [Arguments(
        "TestMap",
        1,
        2,
        "TestMap:(1, 2)")]
    [Arguments(
        "arena",
        -5,
        10,
        "arena:(-5, 10)")]
    [Arguments(
        "dungeon_1",
        100,
        -200,
        "dungeon_1:(100, -200)")]
    [Arguments(
        "",
        int.MaxValue,
        int.MinValue,
        ":(2147483647, -2147483648)")]

    //formatter:on
    public void ILocation_ToString_Instance_Location_ReturnsExpectedFormat(
        string map,
        int x,
        int y,
        string expected)
    {
        // Arrange
        ILocation location = new Location(map, x, y);

        // Act
        var result = location.ToString();

        // Assert
        result.Should()
              .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        "map1",
        0,
        0,
        "map1:(0, 0)")]
    [Arguments(
        "TestMap",
        1,
        2,
        "TestMap:(1, 2)")]
    [Arguments(
        "arena",
        -5,
        10,
        "arena:(-5, 10)")]
    [Arguments(
        "dungeon_1",
        100,
        -200,
        "dungeon_1:(100, -200)")]
    [Arguments(
        "",
        int.MaxValue,
        int.MinValue,
        ":(2147483647, -2147483648)")]

    //formatter:on
    public void ILocation_ToString_Instance_ValueLocation_ReturnsExpectedFormat(
        string map,
        int x,
        int y,
        string expected)
    {
        // Arrange
        var location = new ValueLocation(map, x, y);

        // Act
        var result = location.ToString();

        // Assert
        result.Should()
              .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments("Map1", "Map2", false)]
    [Arguments("map1", "map1", true)]
    [Arguments("TestMap", "testmap", false)]

    //formatter:on
    public void ILocation_ToString_PreservesMapNameCasing(string map1, string map2, bool shouldMatch)
    {
        // Arrange
        ILocation location1 = new Location(map1, 0, 0);
        ILocation location2 = new Location(map2, 0, 0);

        // Act
        var result1 = location1.ToString();
        var result2 = location2.ToString();

        // Assert
        if (shouldMatch)
            result1.Should()
                   .Be(result2);
        else
            result1.Should()
                   .NotBe(result2);
    }

    //formatter:off
    [Test]
    [Arguments(
        "map1",
        0,
        0,
        "map1:(0, 0)")]
    [Arguments(
        "TestMap",
        1,
        2,
        "TestMap:(1, 2)")]
    [Arguments(
        "arena",
        -5,
        10,
        "arena:(-5, 10)")]
    [Arguments(
        "dungeon_1",
        100,
        -200,
        "dungeon_1:(100, -200)")]
    [Arguments(
        "",
        int.MaxValue,
        int.MinValue,
        ":(2147483647, -2147483648)")]

    //formatter:on
    public void ILocation_ToString_Static_Location_ReturnsExpectedFormat(
        string map,
        int x,
        int y,
        string expected)
    {
        // Arrange
        ILocation location = new Location(map, x, y);

        // Act
        var result = ILocation.ToString(location);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void ILocation_ToString_WithNullMap_DoesNotThrow()
    {
        // Arrange
        ILocation location = new Location(null!, 5, 10);

        // Act
        var act = () => location.ToString();

        // Assert
        act.Should()
           .NotThrow();
    }

    [Test]
    public void Location_And_ValueLocation_ToString_ReturnSameFormat()
    {
        // Arrange
        const string MAP = "testMap";
        const int X = 42;
        const int Y = 84;
        var location = new Location(MAP, X, Y);
        var valueLocation = new ValueLocation(MAP, X, Y);

        // Act
        var locationResult = location.ToString();
        var valueLocationResult = valueLocation.ToString();

        // Assert
        locationResult.Should()
                      .Be(valueLocationResult);
    }
}
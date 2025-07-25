#region
using Chaos.Geometry.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class LocationTests
{
    [Test]
    public void Constructor_WithStringAndIPoint_InitializesCorrectly()
    {
        var point = (IPoint)new Point(5, 10);
        var location = new Location("TestMap", point);

        location.Map
                .Should()
                .Be("TestMap");

        location.X
                .Should()
                .Be(5);

        location.Y
                .Should()
                .Be(10);
    }

    [Test]
    public void Constructor_WithStringAndPoint_InitializesCorrectly()
    {
        var point = new Point(5, 10);
        var location = new Location("TestMap", point);

        location.Map
                .Should()
                .Be("TestMap");

        location.X
                .Should()
                .Be(5);

        location.Y
                .Should()
                .Be(10);
    }

    [Test]
    public void Deconstructor_ReturnsCorrectValues()
    {
        var location = new Location("TestMap", 5, 10);
        (var map, var x, var y) = location;

        map.Should()
           .Be("TestMap");

        x.Should()
         .Be(5);

        y.Should()
         .Be(10);
    }

    [Test]
    public void EqualityOperator_ReturnsTrueForSameLocations()
    {
        var location1 = new Location("TestMap", 5, 10);
        ILocation location2 = new Location("TestMap", 5, 10);

        (location1 == location2).Should()
                                .BeTrue();
    }

    [Test]
    public void Equals_ReturnsFalseForDifferentLocations()
    {
        var location1 = new Location("TestMap1", 5, 10);
        ILocation location2 = new Location("TestMap2", 5, 10);

        location1.Equals(location2)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void Equals_ReturnsTrueForSameLocations()
    {
        var location1 = new Location("TestMap", 5, 10);
        ILocation location2 = new Location("TestMap", 5, 10);

        location1.Equals(location2)
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void InequalityOperator_ReturnsFalseForSameLocations()
    {
        var location1 = new Location("TestMap", 5, 10);
        ILocation location2 = new Location("TestMap", 5, 10);

        (location1 != location2).Should()
                                .BeFalse();
    }

    [Test]
    public void InequalityOperator_ReturnsTrueForDifferentLocations()
    {
        var location1 = new Location("TestMap1", 5, 10);
        ILocation location2 = new Location("TestMap2", 5, 10);

        (location1 != location2).Should()
                                .BeTrue();
    }

    [Test]
    public void Location_Constructor_CreatesLocationWithGivenValues()
    {
        // Arrange
        const string MAP = "Map1";
        const int X = 10;
        const int Y = 20;

        // Act
        var location = new Location(MAP, X, Y);

        // Assert
        location.Map
                .Should()
                .Be(MAP);

        location.X
                .Should()
                .Be(X);

        location.Y
                .Should()
                .Be(Y);
    }

    [Test]
    public void Location_Equals_ReturnsFalseWhenComparingWithDifferentType()
    {
        // Arrange
        var location = new Location("Map1", 10, 20);
        var otherObject = new object();

        // Act
        var result = location.Equals(otherObject);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void Location_Equals_ReturnsFalseWhenLocationsAreNotEqual()
    {
        // Arrange
        var location1 = new Location("Map1", 10, 20);
        var location2 = new Location("Map2", 10, 20);

        // Act
        var result = location1.Equals(location2);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void Location_Equals_ReturnsTrueWhenLocationsAreEqual()
    {
        // Arrange
        const string MAP = "Map1";
        const int X = 10;
        const int Y = 20;
        var location1 = new Location(MAP, X, Y);
        var location2 = new Location(MAP, X, Y);

        // Act
        var result = location1.Equals(location2);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Location_From_ReturnsNewLocationWithSameValuesWhenPassedLocationOfDifferentType()
    {
        // Arrange
        ILocation originalLocation = new MockLocation("Map1", 10, 20);

        // Act
        var newLocation = Location.From(originalLocation);

        // Assert
        newLocation.Map
                   .Should()
                   .Be(originalLocation.Map);

        newLocation.X
                   .Should()
                   .Be(originalLocation.X);

        newLocation.Y
                   .Should()
                   .Be(originalLocation.Y);
    }

    [Test]
    public void Location_From_ReturnsSameLocationWhenPassedLocationOfSameType()
    {
        // Arrange
        var originalLocation = new Location("Map1", 10, 20);

        // Act
        var newLocation = Location.From(originalLocation);

        // Assert
        newLocation.Should()
                   .BeEquivalentTo(originalLocation);
    }

    [Test]
    public void Location_GetHashCode_ReturnsConsistentHashCode()
    {
        // Arrange
        var location = new Location("Map1", 10, 20);
        var expectedHashCode = HashCode.Combine(location.X, location.Y, location.Map);

        // Act
        var hashCode1 = location.GetHashCode();
        var hashCode2 = location.GetHashCode();

        // Assert
        hashCode1.Should()
                 .Be(expectedHashCode);

        hashCode2.Should()
                 .Be(expectedHashCode);
    }

    [Test]
    public void Location_TryParse_InvalidInput_ReturnsFalseAndDefaultLocation()
    {
        // Arrange
        const string INPUT = "Invalid input";

        // Act
        var result = Location.TryParse(INPUT, out var location);

        // Assert
        result.Should()
              .BeFalse();

        location.Should()
                .Be(null);
    }

    [Test]
    public void Location_TryParse_ValidInput_ReturnsTrueAndParsesLocation()
    {
        // Arrange
        const string INPUT = "Example: (123, 456)";
        const string EXPECTED_MAP = "Example";
        const int EXPECTED_X = 123;
        const int EXPECTED_Y = 456;

        // Act
        var result = Location.TryParse(INPUT, out var location);

        // Assert
        result.Should()
              .BeTrue();

        location!.Map
                 .Should()
                 .Be(EXPECTED_MAP);

        location.X
                .Should()
                .Be(EXPECTED_X);

        location.Y
                .Should()
                .Be(EXPECTED_Y);
    }

    [Test]
    public void ToString_ReturnsExpectedFormat()
    {
        // Assuming ToString() outputs in the format: "Map: X,Y"
        var location = new Location("TestMap", 5, 10);
        var result = location.ToString();

        result.Should()
              .Be("TestMap:(5, 10)"); // Adjust this based on your actual expected format
    }

    // CustomLocation class for testing Location.From method
    private sealed class MockLocation(string map, int x, int y) : ILocation
    {
        public string Map { get; } = map;
        public int X { get; } = x;
        public int Y { get; } = y;
    }
}
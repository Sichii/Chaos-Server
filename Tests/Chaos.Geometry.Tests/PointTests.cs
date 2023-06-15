using Chaos.Geometry.Abstractions;
using FluentAssertions;
using Xunit;

namespace Chaos.Geometry.Tests;

public sealed class PointTests
{
    [Fact]
    public void Point_Constructor_CreatesPointWithGivenCoordinates()
    {
        // Arrange
        const int X = 10;
        const int Y = 20;

        // Act
        var point = new Point(X, Y);

        // Assert
        point.X.Should().Be(X);
        point.Y.Should().Be(Y);
    }

    [Fact]
    public void Point_Equals_ReturnsFalseWhenComparingWithDifferentType()
    {
        // Arrange
        var point = new Point(10, 20);
        var otherObject = new object();

        // Act
        var result = point.Equals(otherObject);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Point_Equals_ReturnsFalseWhenPointsAreNotEqual()
    {
        // Arrange
        var point1 = new Point(10, 20);
        var point2 = new Point(30, 40);

        // Act
        var result = point1.Equals(point2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Point_Equals_ReturnsTrueWhenPointsAreEqual()
    {
        // Arrange
        const int X = 10;
        const int Y = 20;
        var point1 = new Point(X, Y);
        var point2 = new Point(X, Y);

        // Act
        var result = point1.Equals(point2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Point_From_ReturnsNewPointWithSameValuesWhenPassedPointOfDifferentType()
    {
        // Arrange
        IPoint originalPoint = new MockPoint(10, 20);

        // Act
        var newPoint = Point.From(originalPoint);

        // Assert
        newPoint.X.Should().Be(originalPoint.X);
        newPoint.Y.Should().Be(originalPoint.Y);
    }

    [Fact]
    public void Point_From_ReturnsSamePointWhenPassedPointOfSameType()
    {
        // Arrange
        var originalPoint = new Point(10, 20);

        // Act
        var newPoint = Point.From(originalPoint);

        // Assert
        newPoint.Should().BeEquivalentTo(originalPoint);
    }

    [Fact]
    public void Point_GetHashCode_ReturnsConsistentHashCode()
    {
        // Arrange
        var point = new Point(10, 20);
        const int EXPECTED_HASH_CODE = (10 << 16) + 20;

        // Act
        var hashCode1 = point.GetHashCode();
        var hashCode2 = point.GetHashCode();

        // Assert
        hashCode1.Should().Be(EXPECTED_HASH_CODE);
        hashCode2.Should().Be(EXPECTED_HASH_CODE);
    }

    [Fact]
    public void Point_TryParse_InvalidInput_ReturnsFalseAndDefaultPoint()
    {
        // Arrange
        const string INPUT = "Invalid input";

        // Act
        var result = Point.TryParse(INPUT, out var point);

        // Assert
        result.Should().BeFalse();
        point.Should().Be(default(Point));
    }

    [Fact]
    public void Point_TryParse_ValidInput_ReturnsTrueAndParsesPoint()
    {
        // Arrange
        const string INPUT = "(123, 456)";
        const int EXPECTED_X = 123;
        const int EXPECTED_Y = 456;

        // Act
        var result = Point.TryParse(INPUT, out var point);

        // Assert
        result.Should().BeTrue();
        point.X.Should().Be(EXPECTED_X);
        point.Y.Should().Be(EXPECTED_Y);
    }

    // CustomPoint class for testing Point.From method
    private sealed class MockPoint : IPoint
    {
        public int X { get; }
        public int Y { get; }

        public MockPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
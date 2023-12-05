using Chaos.Geometry.Abstractions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Chaos.Geometry.Tests;

public sealed class CircleTests
{
    [Fact]
    public void ByteTupleToPointConversion_ReturnsExpectedPoint()
    {
        (byte, byte) tuple = (2, 3);

        Point point = tuple;

        point.X
             .Should()
             .Be(2);

        point.Y
             .Should()
             .Be(3);
    }

    [Fact]
    public void Circle_Constructor_CreatesCircleWithGivenCenterAndRadius()
    {
        // Arrange
        var centerMock = new Mock<IPoint>();
        const int RADIUS = 5;

        // Act
        var circle = new Circle(centerMock.Object, RADIUS);

        // Assert
        circle.Center
              .Should()
              .Be(centerMock.Object);

        circle.Radius
              .Should()
              .Be(RADIUS);
    }

    [Fact]
    public void Circle_Equals_ReturnsFalseWhenCirclesAreNotEqual()
    {
        // Arrange
        var center1Mock = new Mock<IPoint>();
        var center2Mock = new Mock<IPoint>();
        const int RADIUS1 = 5;
        const int RADIUS2 = 10;
        var circle1 = new Circle(center1Mock.Object, RADIUS1);
        var circle2 = new Circle(center2Mock.Object, RADIUS2);

        // Act
        var result = circle1.Equals(circle2);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Fact]
    public void Circle_Equals_ReturnsFalseWhenComparingWithDifferentType()
    {
        // Arrange
        var centerMock = new Mock<IPoint>();
        const int RADIUS = 5;
        var circle = new Circle(centerMock.Object, RADIUS);
        var otherObject = new object();

        // Act
        var result = circle.Equals(otherObject);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Fact]
    public void Circle_Equals_ReturnsTrueWhenCirclesAreEqual()
    {
        // Arrange
        var centerMock = new Mock<IPoint>();
        const int RADIUS = 5;
        var circle1 = new Circle(centerMock.Object, RADIUS);
        var circle2 = new Circle(centerMock.Object, RADIUS);

        // Act
        var result = circle1.Equals(circle2);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void Circle_GetHashCode_ReturnsConsistentHashCode()
    {
        // Arrange
        var centerMock = new Mock<IPoint>();
        const int RADIUS = 5;
        var circle = new Circle(centerMock.Object, RADIUS);
        var expectedHashCode = HashCode.Combine(centerMock.Object, RADIUS);

        // Act
        var hashCode1 = circle.GetHashCode();
        var hashCode2 = circle.GetHashCode();

        // Assert
        hashCode1.Should()
                 .Be(expectedHashCode);

        hashCode2.Should()
                 .Be(expectedHashCode);
    }

    [Fact]
    public void EqualityOperator_ReturnsTrueForSamePoints()
    {
        var point1 = new Point(5, 5);
        IPoint point2 = new Point(5, 5);

        var result = point1 == point2;

        result.Should()
              .BeTrue();
    }

    [Fact]
    public void EqualityOperator_WithSameValues_ReturnsTrue()
    {
        // Arrange
        var circle1 = new Circle(new Point(0, 0), 5);
        var circle2 = new Circle(new Point(0, 0), 5);

        // Act
        var result = circle1 == circle2;

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void InequalityOperator_ReturnsFalseForSamePoints()
    {
        var point1 = new Point(5, 5);
        IPoint point2 = new Point(5, 5);

        var result = point1 != point2;

        result.Should()
              .BeFalse();
    }

    [Fact]
    public void InequalityOperator_ReturnsTrueForDifferentPoints()
    {
        var point1 = new Point(5, 5);
        IPoint point2 = new Point(6, 6);

        var result = point1 != point2;

        result.Should()
              .BeTrue();
    }

    [Fact]
    public void InequalityOperator_WithDifferentValues_ReturnsTrue()
    {
        // Arrange
        var circle1 = new Circle(new Point(0, 0), 5);
        var circle2 = new Circle(new Point(1, 1), 1);

        // Act
        var result = circle1 != circle2;

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void IntTupleToPointConversion_ReturnsExpectedPoint()
    {
        var tuple = (500, 1000);

        Point point = tuple;

        point.X
             .Should()
             .Be(500);

        point.Y
             .Should()
             .Be(1000);
    }

    [Fact]
    public void ShortTupleToPointConversion_ReturnsExpectedPoint()
    {
        (short, short) tuple = (-5, -10);

        Point point = tuple;

        point.X
             .Should()
             .Be(-5);

        point.Y
             .Should()
             .Be(-10);
    }

    [Fact]
    public void UShortTupleToPointConversion_ReturnsExpectedPoint()
    {
        (ushort, ushort) tuple = (3000, 4000);

        Point point = tuple;

        point.X
             .Should()
             .Be(3000);

        point.Y
             .Should()
             .Be(4000);
    }
}
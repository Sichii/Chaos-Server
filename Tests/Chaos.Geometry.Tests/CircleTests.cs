using Chaos.Geometry.Abstractions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Chaos.Geometry.Tests;

public sealed class CircleTests
{
    [Fact]
    public void Circle_Constructor_CreatesCircleWithGivenCenterAndRadius()
    {
        // Arrange
        var centerMock = new Mock<IPoint>();
        const int RADIUS = 5;

        // Act
        var circle = new Circle(centerMock.Object, RADIUS);

        // Assert
        circle.Center.Should().Be(centerMock.Object);
        circle.Radius.Should().Be(RADIUS);
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
        result.Should().BeFalse();
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
        result.Should().BeFalse();
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
        result.Should().BeTrue();
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
        hashCode1.Should().Be(expectedHashCode);
        hashCode2.Should().Be(expectedHashCode);
    }
}
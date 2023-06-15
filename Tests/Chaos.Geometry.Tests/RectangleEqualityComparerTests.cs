using Chaos.Geometry.EqualityComparers;
using FluentAssertions;
using Xunit;

namespace Chaos.Geometry.Tests;

public sealed class RectangleEqualityComparerTests
{
    [Fact]
    public void Equals_ReturnsFalse_WhenRectanglesAreNotEqual()
    {
        // Arrange
        var rectangle1 = new Rectangle(
            1,
            2,
            3,
            4);

        var rectangle2 = new Rectangle(
            5,
            6,
            7,
            8);

        // Act
        var result = RectangleEqualityComparer.Instance.Equals(rectangle1, rectangle2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_ReturnsTrue_WhenRectanglesAreEqual()
    {
        // Arrange
        var rectangle1 = new Rectangle(
            1,
            2,
            3,
            4);

        var rectangle2 = new Rectangle(
            1,
            2,
            3,
            4);

        // Act
        var result = RectangleEqualityComparer.Instance.Equals(rectangle1, rectangle2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentHashCode_WhenRectanglesAreNotEqual()
    {
        // Arrange
        var rectangle1 = new Rectangle(
            1,
            2,
            3,
            4);

        var rectangle2 = new Rectangle(
            5,
            6,
            7,
            8);

        // Act
        var hashCode1 = RectangleEqualityComparer.Instance.GetHashCode(rectangle1);
        var hashCode2 = RectangleEqualityComparer.Instance.GetHashCode(rectangle2);

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }

    [Fact]
    public void GetHashCode_ReturnsSameHashCode_WhenRectanglesAreEqual()
    {
        // Arrange
        var rectangle1 = new Rectangle(
            1,
            2,
            3,
            4);

        var rectangle2 = new Rectangle(
            1,
            2,
            3,
            4);

        // Act
        var hashCode1 = RectangleEqualityComparer.Instance.GetHashCode(rectangle1);
        var hashCode2 = RectangleEqualityComparer.Instance.GetHashCode(rectangle2);

        // Assert
        hashCode1.Should().Be(hashCode2);
    }
}
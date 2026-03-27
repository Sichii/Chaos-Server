#region
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class CircleEqualityComparerTests
{
    [Test]
    public void Equals_ReturnsFalse_WhenCirclesAreNotEqual()
    {
        // Arrange
        var circle1 = new Circle(new Point(1, 2), 5);
        var circle2 = new Circle(new Point(3, 4), 6);

        // Act
        var result = CircleEqualityComparer.Instance.Equals(circle1, circle2);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_ReturnsFalse_WhenSameCenterDifferentRadius()
    {
        // Arrange - same center, different radius to hit the radius-differs branch
        var circle1 = new Circle(new Point(1, 2), 5);
        var circle2 = new Circle(new Point(1, 2), 10);

        // Act
        var result = CircleEqualityComparer.Instance.Equals(circle1, circle2);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_ReturnsFalse_WhenXIsNull()
    {
        ICircle? x = null;
        ICircle? y = new Circle(new Point(1, 2), 5);
        var result = CircleEqualityComparer.Instance.Equals(x, y);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_ReturnsFalse_WhenYIsNull()
    {
        ICircle? x = new Circle(new Point(1, 2), 5);
        ICircle? y = null;
        var result = CircleEqualityComparer.Instance.Equals(x, y);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_ReturnsTrue_WhenCirclesAreEqual()
    {
        // Arrange
        var circle1 = new Circle(new Point(1, 2), 5);
        var circle2 = new Circle(new Point(1, 2), 5);

        // Act
        var result = CircleEqualityComparer.Instance.Equals(circle1, circle2);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Equals_ReturnsTrue_WhenSameReference()
    {
        var circle = new Circle(new Point(1, 2), 5);
        var result = CircleEqualityComparer.Instance.Equals(circle, circle);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void GetHashCode_ReturnsDifferentHashCode_WhenCirclesAreNotEqual()
    {
        // Arrange
        var circle1 = new Circle(new Point(1, 2), 5);
        var circle2 = new Circle(new Point(3, 4), 6);

        // Act
        var hashCode1 = CircleEqualityComparer.Instance.GetHashCode(circle1);
        var hashCode2 = CircleEqualityComparer.Instance.GetHashCode(circle2);

        // Assert
        hashCode1.Should()
                 .NotBe(hashCode2);
    }

    [Test]
    public void GetHashCode_ReturnsSameHashCode_WhenCirclesAreEqual()
    {
        // Arrange
        var circle1 = new Circle(new Point(1, 2), 5);
        var circle2 = new Circle(new Point(1, 2), 5);

        // Act
        var hashCode1 = CircleEqualityComparer.Instance.GetHashCode(circle1);
        var hashCode2 = CircleEqualityComparer.Instance.GetHashCode(circle2);

        // Assert
        hashCode1.Should()
                 .Be(hashCode2);
    }
}
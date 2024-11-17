#region
using Chaos.Geometry.EqualityComparers;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class PointEqualityComparerTests
{
    [Test]
    public void Equals_ReturnsFalse_WhenPointsAreNotEqual()
    {
        // Arrange
        var point1 = new Point(1, 2);
        var point2 = new Point(3, 4);

        // Act
        var result = PointEqualityComparer.Instance.Equals(point1, point2);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_ReturnsTrue_WhenPointsAreEqual()
    {
        // Arrange
        var point1 = new Point(1, 2);
        var point2 = new Point(1, 2);

        // Act
        var result = PointEqualityComparer.Instance.Equals(point1, point2);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void GetHashCode_ReturnsDifferentHashCode_WhenPointsAreNotEqual()
    {
        // Arrange
        var point1 = new Point(1, 2);
        var point2 = new Point(3, 4);

        // Act
        var hashCode1 = PointEqualityComparer.Instance.GetHashCode(point1);
        var hashCode2 = PointEqualityComparer.Instance.GetHashCode(point2);

        // Assert
        hashCode1.Should()
                 .NotBe(hashCode2);
    }

    [Test]
    public void GetHashCode_ReturnsSameHashCode_WhenPointsAreEqual()
    {
        // Arrange
        var point1 = new Point(1, 2);
        var point2 = new Point(1, 2);

        // Act
        var hashCode1 = PointEqualityComparer.Instance.GetHashCode(point1);
        var hashCode2 = PointEqualityComparer.Instance.GetHashCode(point2);

        // Assert
        hashCode1.Should()
                 .Be(hashCode2);
    }
}
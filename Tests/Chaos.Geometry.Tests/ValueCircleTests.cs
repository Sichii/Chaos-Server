#region
using Chaos.Geometry.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class ValueCircleTests
{
    [Test]
    public void Constructor_SetsCenterAndRadius()
    {
        IPoint center = new Point(5, 5);
        var vc = new ValueCircle(center, 3);

        vc.Center
          .Should()
          .Be(center);

        vc.Radius
          .Should()
          .Be(3);
    }

    [Test]
    public void Equals_ICircle_DifferentCenter_ReturnsFalse()
    {
        var vc = new ValueCircle(new Point(1, 1), 3);
        ICircle other = new Circle(new Point(9, 9), 3);

        vc.Equals(other)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_ICircle_DifferentRadius_ReturnsFalse()
    {
        IPoint center = new Point(5, 5);
        var vc = new ValueCircle(center, 3);
        ICircle other = new Circle(new Point(5, 5), 10);

        vc.Equals(other)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_ICircle_Null_ReturnsFalse()
    {
        var vc = new ValueCircle(new Point(5, 5), 3);

        vc.Equals(null)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_ICircle_SameCenterAndRadius_ReturnsTrue()
    {
        IPoint center = new Point(5, 5);
        var vc = new ValueCircle(center, 3);
        ICircle other = new Circle(new Point(5, 5), 3);

        vc.Equals(other)
          .Should()
          .BeTrue();
    }

    [Test]
    public void Equals_Object_ICircle_ReturnsTrue()
    {
        var vc = new ValueCircle(new Point(5, 5), 3);
        object other = new Circle(new Point(5, 5), 3);

        vc.Equals(other)
          .Should()
          .BeTrue();
    }

    [Test]
    public void Equals_Object_NonCircle_ReturnsFalse()
    {
        var vc = new ValueCircle(new Point(5, 5), 3);

        // ReSharper disable once SuspiciousTypeConversion.Global
        vc.Equals("not a circle")
          .Should()
          .BeFalse();
    }

    [Test]
    public void ExplicitConversion_FromCircle()
    {
        var circle = new Circle(new Point(2, 3), 7);
        var vc = (ValueCircle)circle;

        vc.Radius
          .Should()
          .Be(7);

        vc.Center
          .Equals(new Point(2, 3))
          .Should()
          .BeTrue();
    }

    [Test]
    public void GetHashCode_SameForEqualCircles()
    {
        var vc1 = new ValueCircle(new Point(5, 5), 3);
        var vc2 = new ValueCircle(new Point(5, 5), 3);

        vc1.GetHashCode()
           .Should()
           .Be(vc2.GetHashCode());
    }

    [Test]
    public void Operator_Equality_ReturnsTrue_ForEqual()
    {
        var vc = new ValueCircle(new Point(1, 1), 5);
        ICircle other = new Circle(new Point(1, 1), 5);

        (vc == other).Should()
                     .BeTrue();
    }

    [Test]
    public void Operator_Inequality_ReturnsTrue_ForDifferent()
    {
        var vc = new ValueCircle(new Point(1, 1), 5);
        ICircle other = new Circle(new Point(9, 9), 5);

        (vc != other).Should()
                     .BeTrue();
    }
}
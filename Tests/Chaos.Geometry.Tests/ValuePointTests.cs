#region
using Chaos.Geometry.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class ValuePointTests
{
    [Test]
    public void Constructor_SetsXAndY()
    {
        var vp = new ValuePoint(3, 7);

        vp.X
          .Should()
          .Be(3);

        vp.Y
          .Should()
          .Be(7);
    }

    [Test]
    public void Deconstruct_ReturnsXAndY()
    {
        var vp = new ValuePoint(4, 8);

        (var x, var y) = vp;

        x.Should()
         .Be(4);

        y.Should()
         .Be(8);
    }

    [Test]
    public void Equals_IPoint_DifferentCoords_ReturnsFalse()
    {
        var vp = new ValuePoint(1, 2);
        IPoint other = new Point(3, 4);

        vp.Equals(other)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_IPoint_Null_ReturnsFalse()
    {
        var vp = new ValuePoint(1, 2);

        vp.Equals(null)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_IPoint_SameCoords_ReturnsTrue()
    {
        var vp = new ValuePoint(1, 2);
        IPoint other = new Point(1, 2);

        vp.Equals(other)
          .Should()
          .BeTrue();
    }

    [Test]
    public void Equals_Object_NonIPoint_ReturnsFalse()
    {
        var vp = new ValuePoint(1, 2);

        // ReSharper disable once SuspiciousTypeConversion.Global
        vp.Equals("not a point")
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_Object_WithIPoint_DelegatesCorrectly()
    {
        var vp = new ValuePoint(1, 2);
        object other = new Point(1, 2);

        vp.Equals(other)
          .Should()
          .BeTrue();
    }

    [Test]
    public void Equals_ValuePoint_Same_ReturnsTrue()
    {
        var vp1 = new ValuePoint(5, 5);
        var vp2 = new ValuePoint(5, 5);

        vp1.Equals(vp2)
           .Should()
           .BeTrue();
    }

    [Test]
    public void From_IPoint_CreatesCorrectValuePoint()
    {
        IPoint source = new Point(5, 10);

        var vp = ValuePoint.From(source);

        vp.X
          .Should()
          .Be(5);

        vp.Y
          .Should()
          .Be(10);
    }

    [Test]
    public void GetHashCode_SameForEqualPoints()
    {
        var vp1 = new ValuePoint(10, 20);
        var vp2 = new ValuePoint(10, 20);

        vp1.GetHashCode()
           .Should()
           .Be(vp2.GetHashCode());
    }

    [Test]
    public void ImplicitConversion_FromByteTuple()
    {
        ValuePoint vp = (1, 2);

        vp.X
          .Should()
          .Be(1);

        vp.Y
          .Should()
          .Be(2);
    }

    [Test]
    public void ImplicitConversion_FromIntTuple()
    {
        ValuePoint vp = (42, 99);

        vp.X
          .Should()
          .Be(42);

        vp.Y
          .Should()
          .Be(99);
    }

    [Test]
    public void ImplicitConversion_FromPoint()
    {
        var point = new Point(7, 8);
        ValuePoint vp = point;

        vp.X
          .Should()
          .Be(7);

        vp.Y
          .Should()
          .Be(8);
    }

    [Test]
    public void ImplicitConversion_FromShortTuple()
    {
        ValuePoint vp = (10, 20);

        vp.X
          .Should()
          .Be(10);

        vp.Y
          .Should()
          .Be(20);
    }

    [Test]
    public void ImplicitConversion_FromUshortTuple()
    {
        ValuePoint vp = (100, 200);

        vp.X
          .Should()
          .Be(100);

        vp.Y
          .Should()
          .Be(200);
    }

    [Test]
    public void Operator_Equality_ReturnsTrue_ForSameCoords()
    {
        var vp = new ValuePoint(1, 2);
        IPoint other = new Point(1, 2);

        (vp == other).Should()
                     .BeTrue();
    }

    [Test]
    public void Operator_Inequality_ReturnsTrue_ForDifferentCoords()
    {
        var vp = new ValuePoint(1, 2);
        IPoint other = new Point(3, 4);

        (vp != other).Should()
                     .BeTrue();
    }

    [Test]
    public void ToString_ReturnsCorrectFormat()
    {
        var vp = new ValuePoint(3, 7);

        vp.ToString()
          .Should()
          .Be("(3, 7)");
    }

    [Test]
    public void TryParse_InvalidString_ReturnsFalse()
    {
        var result = ValuePoint.TryParse("invalid", out _);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryParse_ValidString_ReturnsTrue()
    {
        var result = ValuePoint.TryParse("(10, 20)", out var point);

        result.Should()
              .BeTrue();

        point.X
             .Should()
             .Be(10);

        point.Y
             .Should()
             .Be(20);
    }
}
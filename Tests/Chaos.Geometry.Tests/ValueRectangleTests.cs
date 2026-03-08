#region
using Chaos.Geometry.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class ValueRectangleTests
{
    [Test]
    public void Area_ReturnsWidthTimesHeight()
    {
        var vr = new ValueRectangle(
            0,
            0,
            5,
            3);

        vr.Area
          .Should()
          .Be(15);
    }

    [Test]
    public void Constructor_CenterWidthHeight_SetsProperties()
    {
        IPoint center = new Point(5, 5);
        var vr = new ValueRectangle(center, 3, 3);

        vr.Left
          .Should()
          .Be(4);

        vr.Top
          .Should()
          .Be(4);

        vr.Width
          .Should()
          .Be(3);

        vr.Height
          .Should()
          .Be(3);
    }

    [Test]
    public void Constructor_LeftTopWidthHeight_SetsProperties()
    {
        var vr = new ValueRectangle(
            1,
            2,
            5,
            3);

        vr.Left
          .Should()
          .Be(1);

        vr.Top
          .Should()
          .Be(2);

        vr.Width
          .Should()
          .Be(5);

        vr.Height
          .Should()
          .Be(3);

        vr.Right
          .Should()
          .Be(5);

        vr.Bottom
          .Should()
          .Be(4);
    }

    [Test]
    public void Equals_IRectangle_DifferentValues_ReturnsFalse()
    {
        var vr = new ValueRectangle(
            1,
            2,
            5,
            3);

        IRectangle other = new Rectangle(
            0,
            0,
            10,
            10);

        vr.Equals(other)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_IRectangle_Null_ReturnsFalse()
    {
        var vr = new ValueRectangle(
            1,
            2,
            5,
            3);

        vr.Equals(null)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_IRectangle_SameValues_ReturnsTrue()
    {
        var vr = new ValueRectangle(
            1,
            2,
            5,
            3);

        IRectangle other = new Rectangle(
            1,
            2,
            5,
            3);

        vr.Equals(other)
          .Should()
          .BeTrue();
    }

    [Test]
    public void Equals_Object_IRectangle_ReturnsTrue()
    {
        var vr = new ValueRectangle(
            1,
            2,
            5,
            3);

        object other = new Rectangle(
            1,
            2,
            5,
            3);

        vr.Equals(other)
          .Should()
          .BeTrue();
    }

    [Test]
    public void Equals_Object_NonRectangle_ReturnsFalse()
    {
        var vr = new ValueRectangle(
            1,
            2,
            5,
            3);

        // ReSharper disable once SuspiciousTypeConversion.Global
        vr.Equals("not a rect")
          .Should()
          .BeFalse();
    }

    [Test]
    public void ExplicitConversion_FromRectangle()
    {
        var rect = new Rectangle(
            1,
            2,
            5,
            3);
        var vr = (ValueRectangle)rect;

        vr.Left
          .Should()
          .Be(1);

        vr.Top
          .Should()
          .Be(2);

        vr.Width
          .Should()
          .Be(5);

        vr.Height
          .Should()
          .Be(3);
    }

    [Test]
    public void GetEnumerator_EnumeratesVertices()
    {
        var vr = new ValueRectangle(
            0,
            0,
            2,
            2);
        var points = new List<IPoint>();

        foreach (var point in vr)
            points.Add(point);

        points.Should()
              .HaveCount(4);
    }

    [Test]
    public void GetHashCode_SameForEqualRectangles()
    {
        var vr1 = new ValueRectangle(
            1,
            2,
            5,
            3);

        var vr2 = new ValueRectangle(
            1,
            2,
            5,
            3);

        vr1.GetHashCode()
           .Should()
           .Be(vr2.GetHashCode());
    }

    [Test]
    public void Vertices_ContainsFourCorners()
    {
        var vr = new ValueRectangle(
            0,
            0,
            3,
            2);

        vr.Vertices
          .Should()
          .HaveCount(4);

        vr.Vertices[0]
          .Should()
          .Be(new Point(0, 0));

        vr.Vertices[1]
          .Should()
          .Be(new Point(2, 0));

        vr.Vertices[2]
          .Should()
          .Be(new Point(2, 1));

        vr.Vertices[3]
          .Should()
          .Be(new Point(0, 1));
    }
}
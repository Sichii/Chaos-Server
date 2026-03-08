#region
using Chaos.Geometry.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class ValuePolygonTests
{
    [Test]
    public void Constructor_Default_CreatesEmptyPolygon()
    {
        var vp = new ValuePolygon();

        vp.Vertices
          .Should()
          .BeEmpty();
    }

    [Test]
    public void Constructor_WithVertices_SetsVertices()
    {
        IPoint[] vertices =
        [
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1)
        ];
        var vp = new ValuePolygon(vertices);

        vp.Vertices
          .Should()
          .HaveCount(3);
    }

    [Test]
    public void Equals_IPolygon_DifferentCount_ReturnsFalse()
    {
        var vp = new ValuePolygon(
            [
                new Point(0, 0),
                new Point(1, 0),
                new Point(1, 1)
            ]);

        IPolygon other = new Polygon(
            [
                new Point(0, 0),
                new Point(1, 0)
            ]);

        vp.Equals(other)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_IPolygon_DifferentVertices_ReturnsFalse()
    {
        var vp = new ValuePolygon(
            [
                new Point(0, 0),
                new Point(1, 0),
                new Point(1, 1)
            ]);

        IPolygon other = new Polygon(
            [
                new Point(9, 9),
                new Point(8, 8),
                new Point(7, 7)
            ]);

        vp.Equals(other)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_IPolygon_Null_ReturnsFalse()
    {
        var vp = new ValuePolygon(
            [
                new Point(0, 0),
                new Point(1, 0),
                new Point(1, 1)
            ]);

        vp.Equals(null)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_IPolygon_RotatedVertices_ReturnsTrue()
    {
        var vp = new ValuePolygon(
            [
                new Point(0, 0),
                new Point(1, 0),
                new Point(1, 1)
            ]);

        IPolygon rotated = new Polygon(
            [
                new Point(1, 0),
                new Point(1, 1),
                new Point(0, 0)
            ]);

        vp.Equals(rotated)
          .Should()
          .BeTrue();
    }

    [Test]
    public void Equals_IPolygon_SameVertices_ReturnsTrue()
    {
        IPoint[] vertices =
        [
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1)
        ];
        var vp = new ValuePolygon(vertices);
        IPolygon other = new Polygon(vertices);

        vp.Equals(other)
          .Should()
          .BeTrue();
    }

    [Test]
    public void Equals_Object_IPolygon_ReturnsTrue()
    {
        IPoint[] vertices =
        [
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1)
        ];
        var vp = new ValuePolygon(vertices);
        object other = new Polygon(vertices);

        vp.Equals(other)
          .Should()
          .BeTrue();
    }

    [Test]
    public void Equals_Object_NonPolygon_ReturnsFalse()
    {
        var vp = new ValuePolygon(
            [
                new Point(0, 0),
                new Point(1, 0),
                new Point(1, 1)
            ]);

        // ReSharper disable once SuspiciousTypeConversion.Global
        vp.Equals("not a polygon")
          .Should()
          .BeFalse();
    }

    [Test]
    public void ExplicitConversion_FromPolygon()
    {
        var poly = new Polygon(
            [
                new Point(0, 0),
                new Point(1, 0),
                new Point(1, 1)
            ]);

        var vp = (ValuePolygon)poly;

        vp.Vertices
          .Should()
          .HaveCount(3);
    }

    [Test]
    public void GetEnumerator_EnumeratesVertices()
    {
        IPoint[] vertices =
        [
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1)
        ];
        var vp = new ValuePolygon(vertices);
        var points = new List<IPoint>();

        foreach (var point in vp)
            points.Add(point);

        points.Should()
              .HaveCount(3);
    }

    [Test]
    public void GetHashCode_SameForEqualPolygons()
    {
        IPoint[] vertices =
        [
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1)
        ];
        var vp1 = new ValuePolygon(vertices);
        var vp2 = new ValuePolygon(vertices);

        vp1.GetHashCode()
           .Should()
           .Be(vp2.GetHashCode());
    }
}
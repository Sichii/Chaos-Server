#region
using Chaos.Geometry;
using FluentAssertions;
using Chaos.Geometry.Abstractions;

// ReSharper disable ArrangeAttributes
#endregion

namespace Chaos.Extensions.Geometry.Tests;

public sealed class PolygonExtensionsTests
{
    [Test]
    public void Contains_IPolygon_IPoint_Works_With_IPoint_Argument()
    {
        IPolygon poly = new Polygon(
            [
                new Point(0, 0),
                new Point(0, 4),
                new Point(4, 4),
                new Point(4, 0)
            ]);

        IPoint inside = new Point(2, 2);
        IPoint outside = new Point(5, 5);
        IPoint boundary = new Point(2, 4);

        poly.ContainsPoint(inside)
            .Should()
            .BeTrue();

        poly.ContainsPoint(outside)
            .Should()
            .BeFalse();

        poly.ContainsPoint(boundary)
            .Should()
            .BeTrue();
    }

    [Test]
    public void Contains_IPolygon_Point_FastFail_YBelowMin_And_YAboveMax()
    {
        IPolygon polygon = new Polygon(
            [
                new Point(0, 0),
                new Point(0, 4),
                new Point(4, 4),
                new Point(4, 0)
            ]);

        polygon.ContainsPoint(new Point(2, -1))
               .Should()
               .BeFalse();

        polygon.ContainsPoint(new Point(2, 5))
               .Should()
               .BeFalse();
    }

    //formatter:off
    [Test]
    [Arguments(2, 2, true)]
    [Arguments(5, 5, false)]
    public void Contains_Should_Handle_Reversed_Vertices(int x, int y, bool expected)

        //formatter:on
    {
        var cw = new[]
        {
            new Point(0, 0),
            new Point(0, 4),
            new Point(4, 4),
            new Point(4, 0)
        };

        var ccw = Enumerable.Reverse(cw)
                            .ToArray();

        var polyCw = new Polygon(cw.Cast<IPoint>());
        var polyCcw = new Polygon(ccw.Cast<IPoint>());
        var pt = new Point(x, y);

        var insideCw = polyCw.ContainsPoint(pt);
        var insideCcw = polyCcw.ContainsPoint(pt);

        insideCw.Should()
                .Be(expected);

        insideCcw.Should()
                 .Be(expected);
    }

    [Test]
    public void Contains_Should_Return_False_If_Point_Is_Outside_Polygon()
    {
        // Arrange
        var polygon = new Polygon(
            [
                new Point(0, 0),
                new Point(0, 4),
                new Point(4, 4),
                new Point(4, 0)
            ]);

        var point = new Point(5, 5);

        // Act
        var result = polygon.ContainsPoint(point);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void Contains_Should_Return_True_If_Point_Is_Inside_Polygon()
    {
        // Arrange
        var polygon = new Polygon(
            [
                new Point(0, 0),
                new Point(0, 4),
                new Point(4, 4),
                new Point(4, 0)
            ]);

        var point = new Point(2, 2);

        // Act
        var result = polygon.ContainsPoint(point);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Contains_Should_Return_True_If_Point_Is_On_Polygon_Boundary()
    {
        // Arrange
        var polygon = new Polygon(
            [
                new Point(0, 0),
                new Point(0, 4),
                new Point(4, 4),
                new Point(4, 0)
            ]);

        var point = new Point(2, 4);

        // Act
        var result = polygon.ContainsPoint(point);

        // Assert
        result.Should()
              .BeTrue();
    }

    //formatter:off
    [Test]
    [Arguments(5, 5, false)]
    [Arguments(2, 2, true)]
    [Arguments(2, 4, true)] // boundary
    [Arguments(4, 0, false)] // vertex behavior per pnpoly
    public void Contains_ValuePolygon_Point_Should_Work(int x, int y, bool expected)

        //formatter:on
    {
        var poly = new ValuePolygon(
            [
                new Point(0, 0),
                new Point(0, 4),
                new Point(4, 4),
                new Point(4, 0)
            ]);

        var result = poly.ContainsPoint(new Point(x, y));

        result.Should()
              .Be(expected);
    }

    [Test]
    public void Contains_ValuePolygon_Point_XConjunct_Fails_When_AllVertexX_Greater_Than_PointX()
    {
        var poly = new ValuePolygon(
            [
                new Point(1, 0),
                new Point(1, 4),
                new Point(3, 4),
                new Point(3, 0)
            ]);

        poly.ContainsPoint(new Point(0, 2))
            .Should()
            .BeFalse();
    }

    [Test]
    public void Contains_ValuePolygon_Point_XorFlip_Comparison_False_Path_Is_Exercised()
    {
        var poly = new ValuePolygon(
            [
                new Point(0, 0),
                new Point(4, 0),
                new Point(4, 4),
                new Point(0, 4)
            ]);

        // For edge (0,0)->(4,4) equivalent along outline, choose a point where crossingX == 2 and point.X == 1
        // Ensures (crossingX < point.X) is false on that iteration, while other edges still flip to yield inside
        poly.ContainsPoint(new Point(1, 2))
            .Should()
            .BeTrue();
    }

    //formatter:off
    [Test]
    [Arguments(5, 5, false)]
    [Arguments(2, 2, true)]
    [Arguments(2, 4, true)] // boundary
    [Arguments(0, 0, false)] // vertex behavior per pnpoly
    public void Contains_ValuePolygon_ValuePoint_Should_Work(int x, int y, bool expected)

        //formatter:on
    {
        var poly = new ValuePolygon(
            [
                new Point(0, 0),
                new Point(0, 4),
                new Point(4, 4),
                new Point(4, 0)
            ]);

        var result = poly.ContainsPoint(new ValuePoint(x, y));

        result.Should()
              .Be(expected);
    }

    [Test]
    public void Contains_ValuePolygon_ValuePoint_XorFlip_Comparison_True_Path_Is_Exercised()
    {
        var poly = new ValuePolygon(
            [
                new Point(0, 0),
                new Point(4, 0),
                new Point(4, 4),
                new Point(0, 4)
            ]);

        // For diagonal-like crossing computation, pick a point where crossingX (2) < point.X (3) for some edge
        poly.ContainsPoint(new ValuePoint(3, 2))
            .Should()
            .BeTrue();
    }

    [Test]
    public void GetOutline_Should_Generate_Points_Along_Polygon_Outline()
    {
        // Arrange
        var polygon = new Polygon(
            [
                new Point(0, 0),
                new Point(0, 4),
                new Point(4, 4),
                new Point(4, 0)
            ]);

        var expectedPoints = new[]
        {
            new Point(0, 0),
            new Point(0, 1),
            new Point(0, 2),
            new Point(0, 3),
            new Point(0, 4),
            new Point(1, 4),
            new Point(2, 4),
            new Point(3, 4),
            new Point(4, 4),
            new Point(4, 3),
            new Point(4, 2),
            new Point(4, 1),
            new Point(4, 0),
            new Point(3, 0),
            new Point(2, 0),
            new Point(1, 0)
        };

        // Act
        var result = polygon.GetOutline();

        // Assert
        result.Should()
              .BeEquivalentTo(expectedPoints);
    }

    [Test]
    public void GetOutline_ValuePolygon_Should_Match_Expected()
    {
        var poly = new ValuePolygon(
            [
                new Point(0, 0),
                new Point(0, 3),
                new Point(3, 3),
                new Point(3, 0)
            ]);

        var expected = new[]
        {
            new Point(0, 0),
            new Point(0, 1),
            new Point(0, 2),
            new Point(0, 3),
            new Point(1, 3),
            new Point(2, 3),
            new Point(3, 3),
            new Point(3, 2),
            new Point(3, 1),
            new Point(3, 0),
            new Point(2, 0),
            new Point(1, 0)
        };

        var outline = poly.GetOutline();

        outline.Should()
               .BeEquivalentTo(expected);
    }
}
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Geometry.Tests;

public sealed class PolygonExtensionsTests
{
    [Fact]
    public void Contains_Should_Return_False_If_Point_Is_Outside_Polygon()
    {
        // Arrange
        var polygon = new Polygon(
            new IPoint[]
            {
                new Point(0, 0),
                new Point(0, 4),
                new Point(4, 4),
                new Point(4, 0)
            });

        var point = new Point(5, 5);

        // Act
        var result = polygon.Contains(point);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Fact]
    public void Contains_Should_Return_True_If_Point_Is_Inside_Polygon()
    {
        // Arrange
        var polygon = new Polygon(
            new IPoint[]
            {
                new Point(0, 0),
                new Point(0, 4),
                new Point(4, 4),
                new Point(4, 0)
            });

        var point = new Point(2, 2);

        // Act
        var result = polygon.Contains(point);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void Contains_Should_Return_True_If_Point_Is_On_Polygon_Boundary()
    {
        // Arrange
        var polygon = new Polygon(
            new IPoint[]
            {
                new Point(0, 0),
                new Point(0, 4),
                new Point(4, 4),
                new Point(4, 0)
            });

        var point = new Point(2, 4);

        // Act
        var result = polygon.Contains(point);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void GetOutline_Should_Generate_Points_Along_Polygon_Outline()
    {
        // Arrange
        var polygon = new Polygon(
            new IPoint[]
            {
                new Point(0, 0),
                new Point(0, 4),
                new Point(4, 4),
                new Point(4, 0)
            });

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
}
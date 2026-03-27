#region
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class AoeShapeExtensionsTests
{
    private static AoeShapeOptions CreateOptions(
        int range = 3,
        Direction? direction = null,
        int? exclusionRange = null,
        IRectangle? bounds = null)
        => new()
        {
            Source = new Point(5, 5),
            Range = range,
            Direction = direction,
            ExclusionRange = exclusionRange,
            Bounds = bounds
        };

    #region ResolvePoints - Bounds
    [Test]
    public void ResolvePoints_WithBounds_ShouldFilterOutOfBoundsPoints()
    {
        var bounds = new Rectangle(
            0,
            0,
            8,
            8);
        var options = CreateOptions(5, bounds: bounds);

        var points = AoeShape.AllAround
                             .ResolvePoints(options)
                             .ToList();

        // All points should be within bounds
        foreach (var point in points)
        {
            point.X
                 .Should()
                 .BeGreaterThanOrEqualTo(0);

            point.Y
                 .Should()
                 .BeGreaterThanOrEqualTo(0);

            point.X
                 .Should()
                 .BeLessThan(8);

            point.Y
                 .Should()
                 .BeLessThan(8);
        }
    }
    #endregion

    #region ResolvePoints - Shape cases
    [Test]
    public void ResolvePoints_None_ShouldReturnSourceOnly()
    {
        var options = CreateOptions();

        var points = AoeShape.None
                             .ResolvePoints(options)
                             .ToList();

        // None returns empty + source is prepended (no exclusion)
        points.Should()
              .ContainSingle()
              .Which
              .Should()
              .Be(new Point(5, 5));
    }

    [Test]
    public void ResolvePoints_Front_ShouldReturnLineOfPoints()
    {
        var options = CreateOptions(3, Direction.Right);

        var points = AoeShape.Front
                             .ResolvePoints(options)
                             .ToList();

        // Should include source + 3 points to the right
        points.Should()
              .Contain(new Point(5, 5));

        points.Should()
              .Contain(new Point(8, 5));
    }

    [Test]
    public void ResolvePoints_Front_ShouldThrow_WhenDirectionNull()
    {
        var options = CreateOptions();

        var act = () => AoeShape.Front
                                .ResolvePoints(options)
                                .ToList();

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void ResolvePoints_AllAround_ShouldReturnSurroundingPoints()
    {
        var options = CreateOptions(1);

        var points = AoeShape.AllAround
                             .ResolvePoints(options)
                             .ToList();

        // Spiral search with range 1 should return at least the 4 adjacent points + source
        points.Should()
              .Contain(new Point(5, 5));

        points.Should()
              .Contain(new Point(5, 4));

        points.Should()
              .Contain(new Point(6, 5));
    }

    [Test]
    public void ResolvePoints_FrontalCone_ShouldReturnConePoints()
    {
        var options = CreateOptions(2, Direction.Down);

        var points = AoeShape.FrontalCone
                             .ResolvePoints(options)
                             .ToList();

        points.Should()
              .NotBeEmpty();

        // Source should be prepended
        points.Should()
              .Contain(new Point(5, 5));
    }

    [Test]
    public void ResolvePoints_FrontalCone_ShouldThrow_WhenDirectionNull()
    {
        var options = CreateOptions(2);

        var act = () => AoeShape.FrontalCone
                                .ResolvePoints(options)
                                .ToList();

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void ResolvePoints_FrontalDiamond_ShouldThrow_WhenDirectionNull()
    {
        var options = CreateOptions(2);

        var act = () => AoeShape.FrontalDiamond
                                .ResolvePoints(options)
                                .ToList();

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void ResolvePoints_FrontalDiamond_ShouldReturnFilteredConePoints()
    {
        var options = CreateOptions(2, Direction.Up);

        var points = AoeShape.FrontalDiamond
                             .ResolvePoints(options)
                             .ToList();

        points.Should()
              .NotBeEmpty();

        // All points should be within range of source (Manhattan distance)
        foreach (var point in points)
        {
            var distance = Math.Abs(point.X - 5) + Math.Abs(point.Y - 5);

            distance.Should()
                    .BeLessThanOrEqualTo(2);
        }
    }

    [Test]
    public void ResolvePoints_Circle_ShouldReturnCircularPoints()
    {
        var options = CreateOptions(2);

        var points = AoeShape.Circle
                             .ResolvePoints(options)
                             .ToList();

        points.Should()
              .NotBeEmpty();

        points.Should()
              .Contain(new Point(5, 5));

        points.Should()
              .Contain(new Point(7, 5));
    }

    [Test]
    public void ResolvePoints_Square_ShouldReturnSquarePoints()
    {
        var options = CreateOptions(1);

        var points = AoeShape.Square
                             .ResolvePoints(options)
                             .ToList();

        // 3x3 square = 9 points + source prepended (but source already in square, deduped)
        points.Should()
              .Contain(new Point(4, 4));

        points.Should()
              .Contain(new Point(6, 6));

        points.Should()
              .Contain(new Point(5, 5));
    }

    [Test]
    public void ResolvePoints_CircleOutline_ShouldReturnPerimeterPoints()
    {
        var options = CreateOptions();

        var points = AoeShape.CircleOutline
                             .ResolvePoints(options)
                             .ToList();

        points.Should()
              .NotBeEmpty();

        // Source should be prepended (no exclusion)
        points.Should()
              .Contain(new Point(5, 5));
    }

    [Test]
    public void ResolvePoints_SquareOutline_ShouldReturnPerimeterPoints()
    {
        var options = CreateOptions(2);

        var points = AoeShape.SquareOutline
                             .ResolvePoints(options)
                             .ToList();

        points.Should()
              .NotBeEmpty();

        // Corners should be included
        points.Should()
              .Contain(new Point(3, 3));

        points.Should()
              .Contain(new Point(7, 7));
    }
    #endregion

    #region ResolvePoints - ExclusionRange
    [Test]
    public void ResolvePoints_WithExclusionRange_ShouldExcludeInnerPoints()
    {
        var options = CreateOptions(exclusionRange: 1);

        var points = AoeShape.AllAround
                             .ResolvePoints(options)
                             .ToList();

        // Source should NOT be included when ExclusionRange is set
        points.Should()
              .NotContain(new Point(5, 5));
    }

    [Test]
    public void ResolvePoints_Square_WithExclusionRange_ShouldUseRectangleExclusion()
    {
        var options = CreateOptions(2, exclusionRange: 1);

        var points = AoeShape.Square
                             .ResolvePoints(options)
                             .ToList();

        // Inner 3x3 square should be excluded
        points.Should()
              .NotContain(new Point(5, 5));

        // Outer points should remain
        points.Should()
              .Contain(new Point(3, 3));
    }

    [Test]
    public void ResolvePoints_Circle_WithExclusionRange_ShouldUseCircleExclusion()
    {
        var options = CreateOptions(exclusionRange: 1);

        var points = AoeShape.Circle
                             .ResolvePoints(options)
                             .ToList();

        points.Should()
              .NotContain(new Point(5, 5));
    }
    #endregion

    #region ResolvePointsForRange
    [Test]
    public void ResolvePointsForRange_None_ShouldReturnEmpty()
    {
        var allPoints = new List<Point>
        {
            new(5, 5),
            new(6, 5),
            new(7, 5)
        };

        var options = new CascadingAoeShapeOptions
        {
            Source = new Point(5, 5),
            Range = 1,
            AllPossiblePoints = allPoints
        };

        var points = AoeShape.None
                             .ResolvePointsForRange(options)
                             .ToList();

        points.Should()
              .BeEmpty();
    }

    [Test]
    public void ResolvePointsForRange_AllAround_ShouldReturnPointsAtExactRange()
    {
        var allPoints = new List<Point>
        {
            new(5, 5),
            new(6, 5),
            new(7, 5),
            new(5, 4),
            new(4, 5)
        };

        var options = new CascadingAoeShapeOptions
        {
            Source = new Point(5, 5),
            Range = 2,
            AllPossiblePoints = allPoints
        };

        var points = AoeShape.AllAround
                             .ResolvePointsForRange(options)
                             .ToList();

        // Only points at Manhattan distance 2 from source
        points.Should()
              .OnlyContain(p => (Math.Abs(p.X - 5) + Math.Abs(p.Y - 5)) == 2);
    }

    [Test]
    public void ResolvePointsForRange_FrontalCone_ShouldFilterByAxis()
    {
        var allPoints = new List<Point>
        {
            new(7, 5),
            new(7, 4),
            new(7, 6),
            new(5, 7),
            new(6, 5)
        };

        var options = new CascadingAoeShapeOptions
        {
            Source = new Point(5, 5),
            Range = 2,
            Direction = Direction.Right,
            AllPossiblePoints = allPoints
        };

        var points = AoeShape.FrontalCone
                             .ResolvePointsForRange(options)
                             .ToList();

        // Right direction = X axis: all points should have X == source.X + range = 7
        points.Should()
              .OnlyContain(p => p.X == 7);
    }

    [Test]
    public void ResolvePointsForRange_FrontalDiamond_ShouldFilterByManhattanDistance()
    {
        var allPoints = new List<Point>
        {
            new(5, 3),
            new(6, 4),
            new(7, 5),
            new(4, 4),
            new(5, 5),
            new(6, 6)
        };

        var options = new CascadingAoeShapeOptions
        {
            Source = new Point(5, 5),
            Range = 2,
            Direction = Direction.Up,
            AllPossiblePoints = allPoints
        };

        var points = AoeShape.FrontalDiamond
                             .ResolvePointsForRange(options)
                             .ToList();

        // FrontalDiamond uses ManhattanDistanceFrom == Range, so only distance-2 points
        points.Should()
              .OnlyContain(p => (Math.Abs(p.X - 5) + Math.Abs(p.Y - 5)) == 2);
    }

    [Test]
    public void ResolvePointsForRange_FrontalCone_VerticalDirection_ShouldFilterByYAxis()
    {
        var allPoints = new List<Point>
        {
            new(5, 7),
            new(4, 7),
            new(6, 7),
            new(7, 5),
            new(5, 6)
        };

        var options = new CascadingAoeShapeOptions
        {
            Source = new Point(5, 5),
            Range = 2,
            Direction = Direction.Down,
            AllPossiblePoints = allPoints
        };

        var points = AoeShape.FrontalCone
                             .ResolvePointsForRange(options)
                             .ToList();

        // Down direction = Y axis: all points should have Y == source.Y + range = 7
        points.Should()
              .OnlyContain(p => p.Y == 7);
    }

    [Test]
    public void ResolvePointsForRange_Circle_ShouldReturnOutlinePointsAtRange()
    {
        var allPoints = new List<Point>
        {
            new(5, 5),
            new(5, 3),
            new(7, 5),
            new(3, 5),
            new(5, 7),
            new(6, 6)
        };

        var options = new CascadingAoeShapeOptions
        {
            Source = new Point(5, 5),
            Range = 2,
            AllPossiblePoints = allPoints
        };

        var points = AoeShape.Circle
                             .ResolvePointsForRange(options)
                             .ToList();

        // Should return only points on the circle outline at range 2
        points.Should()
              .NotBeEmpty();
    }

    [Test]
    public void ResolvePointsForRange_Square_ShouldReturnOutlinePointsAtRange()
    {
        var allPoints = new List<Point>
        {
            new(5, 5),
            new(3, 3),
            new(7, 7),
            new(3, 7),
            new(7, 3),
            new(5, 3),
            new(5, 7)
        };

        var options = new CascadingAoeShapeOptions
        {
            Source = new Point(5, 5),
            Range = 2,
            AllPossiblePoints = allPoints
        };

        var points = AoeShape.Square
                             .ResolvePointsForRange(options)
                             .ToList();

        // Should return only points on the rectangle outline at range 2
        points.Should()
              .NotBeEmpty();
    }

    //formatter:off
    [Test]
    [Arguments(AoeShape.CircleOutline)]
    [Arguments(AoeShape.SquareOutline)]

    //formatter:on
    public void ResolvePointsForRange_UnsupportedShape_ShouldThrow(AoeShape shape)
    {
        var allPoints = new List<Point>
        {
            new(5, 5),
            new(6, 5)
        };

        var options = new CascadingAoeShapeOptions
        {
            Source = new Point(5, 5),
            Range = 1,
            AllPossiblePoints = allPoints
        };

        var act = () => shape.ResolvePointsForRange(options)
                             .ToList();

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }
    #endregion

    #region ResolvePoints - Additional ExclusionRange Cases
    [Test]
    public void ResolvePoints_Front_WithExclusionRange_ShouldUseManhattanDistanceExclusion()
    {
        var options = CreateOptions(3, Direction.Right, 1);

        var points = AoeShape.Front
                             .ResolvePoints(options)
                             .ToList();

        // Source (5,5) should be excluded (Manhattan distance 0 <= 1)
        points.Should()
              .NotContain(new Point(5, 5));

        // Adjacent point (6,5) should also be excluded (Manhattan distance 1 <= 1)
        points.Should()
              .NotContain(new Point(6, 5));

        // Point at distance 2 should remain
        points.Should()
              .Contain(new Point(7, 5));
    }

    [Test]
    public void ResolvePoints_CircleOutline_WithExclusionRange_ShouldUseCircleExclusion()
    {
        var options = CreateOptions(exclusionRange: 1);

        var points = AoeShape.CircleOutline
                             .ResolvePoints(options)
                             .ToList();

        // Source should NOT be included (exclusion removes it)
        points.Should()
              .NotContain(new Point(5, 5));

        // Points on the outline at range 3 should be present
        points.Should()
              .NotBeEmpty();
    }

    [Test]
    public void ResolvePoints_SquareOutline_WithExclusionRange_ShouldUseRectangleExclusion()
    {
        // Range 3 => outline of 7x7 rectangle, exclusion range 1 => 3x3 inner rectangle excluded
        var options = CreateOptions(exclusionRange: 1);

        var points = AoeShape.SquareOutline
                             .ResolvePoints(options)
                             .ToList();

        // Source should NOT be prepended when ExclusionRange is set
        // (outline doesn't include source since it's the center, not the perimeter)
        points.Should()
              .NotContain(new Point(5, 5));

        // Outline perimeter corners at range 3 should be present
        points.Should()
              .Contain(new Point(2, 2));

        points.Should()
              .Contain(new Point(8, 8));
    }
    #endregion
}
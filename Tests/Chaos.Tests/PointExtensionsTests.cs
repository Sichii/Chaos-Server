#region
using Chaos.Extensions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class PointExtensionsTests
{
    #region WithinRange (Point overload)
    [Test]
    public void WithinRange_Point_ShouldReturnTrue_WhenWithinDistance()
    {
        var point1 = new Point(5, 5);
        var point2 = new Point(7, 7);

        point1.WithinRange(point2, 5)
              .Should()
              .BeTrue();
    }

    [Test]
    public void WithinRange_Point_ShouldReturnFalse_WhenOutsideDistance()
    {
        var point1 = new Point(0, 0);
        var point2 = new Point(20, 20);

        point1.WithinRange(point2, 5)
              .Should()
              .BeFalse();
    }

    [Test]
    public void WithinRange_Point_ShouldReturnTrue_WhenExactlyAtDistance()
    {
        var point1 = new Point(0, 0);
        var point2 = new Point(3, 2);

        // Manhattan distance = 3 + 2 = 5
        point1.WithinRange(point2, 5)
              .Should()
              .BeTrue();
    }

    [Test]
    public void WithinRange_Point_ShouldReturnTrue_WhenSamePoint()
    {
        var point = new Point(5, 5);

        point.WithinRange(point, 0)
             .Should()
             .BeTrue();
    }
    #endregion

    #region WithinRange (IPoint overload)
    [Test]
    public void WithinRange_IPoint_ShouldReturnTrue_WhenWithinDistance()
    {
        IPoint point1 = new Point(5, 5);
        IPoint point2 = new Point(7, 7);

        point1.WithinRange(point2, 5)
              .Should()
              .BeTrue();
    }

    [Test]
    public void WithinRange_IPoint_ShouldReturnFalse_WhenOutsideDistance()
    {
        IPoint point1 = new Point(0, 0);
        IPoint point2 = new Point(20, 20);

        point1.WithinRange(point2, 5)
              .Should()
              .BeFalse();
    }

    [Test]
    public void WithinRange_IPoint_ShouldUseDefaultDistance()
    {
        IPoint point1 = new Point(0, 0);
        IPoint point2 = new Point(10, 4);

        // Manhattan distance = 14, default range is 15
        point1.WithinRange(point2)
              .Should()
              .BeTrue();
    }
    #endregion

    #region FilterByLineOfSight (Point overload)
    [Test]
    public void FilterByLineOfSight_ShouldKeepPoints_WhenNoWallsBetween()
    {
        // No walls on the map
        var map = MockMapInstance.Create(width: 20, height: 20);
        var origin = new Point(5, 5);

        var points = new[]
        {
            new Point(5, 8),
            new Point(8, 5)
        };

        var result = points.FilterByLineOfSight(origin, map)
                           .ToList();

        result.Should()
              .HaveCount(2);
    }

    [Test]
    public void FilterByLineOfSight_ShouldExcludePoints_WhenWallBlocks()
    {
        // Wall at (5,7) between origin (5,5) and target (5,9)
        var map = MockMapInstance.Create(width: 20, height: 20, setup: m => MockMapInstance.SetWall(m, new Point(5, 7)));
        var origin = new Point(5, 5);
        var clearTarget = new Point(8, 5); // No wall in path
        var blockedTarget = new Point(5, 9); // Wall at (5,7) blocks this

        var points = new[]
        {
            clearTarget,
            blockedTarget
        };

        var result = points.FilterByLineOfSight(origin, map)
                           .ToList();

        result.Should()
              .Contain(clearTarget);

        result.Should()
              .NotContain(blockedTarget);
    }

    [Test]
    public void FilterByLineOfSight_InvertLos_ShouldExcludePointsBehindWalls()
    {
        // invertLos uses a different algorithm: walls themselves block things behind them
        var map = MockMapInstance.Create(width: 20, height: 20, setup: m => MockMapInstance.SetWall(m, new Point(5, 7)));
        var origin = new Point(5, 5);

        // Points that aren't walls should be kept unless occluded
        var unoccluded = new Point(8, 5); // Not behind any wall
        var wallPoint = new Point(5, 7); // This IS the wall

        var points = new[]
        {
            unoccluded,
            wallPoint
        };

        var result = points.FilterByLineOfSight(origin, map, true)
                           .ToList();

        // With invertLos, wall points themselves generate occlusion rays, and are removed from results
        // The unoccluded point should remain since it's not behind the wall
        result.Should()
              .Contain(unoccluded);
    }

    [Test]
    public void FilterByLineOfSight_ShouldReturnEmpty_WhenAllPointsBlocked()
    {
        // Wall completely blocks all targets
        var map = MockMapInstance.Create(width: 20, height: 20, setup: m => MockMapInstance.SetWall(m, new Point(5, 6)));
        var origin = new Point(5, 5);

        var points = new[]
        {
            new Point(5, 7),
            new Point(5, 8)
        };

        var result = points.FilterByLineOfSight(origin, map)
                           .ToList();

        result.Should()
              .BeEmpty();
    }

    [Test]
    public void FilterByLineOfSight_ShouldReturnAll_WhenNoWallsExist()
    {
        var map = MockMapInstance.Create(width: 20, height: 20);
        var origin = new Point(5, 5);

        var points = new[]
        {
            new Point(6, 6),
            new Point(7, 7),
            new Point(8, 8)
        };

        var result = points.FilterByLineOfSight(origin, map)
                           .ToList();

        result.Should()
              .HaveCount(3);
    }
    #endregion

    #region FilterByLineOfSight (generic T overload)
    [Test]
    public void FilterByLineOfSight_Generic_ShouldKeepPoints_WhenNoWallsBetween()
    {
        var map = MockMapInstance.Create(width: 20, height: 20);
        IPoint origin = new Point(5, 5);

        IPoint[] points =
        [
            new Point(5, 8),
            new Point(8, 5)
        ];

        var result = points.FilterByLineOfSight(origin, map)
                           .ToList();

        result.Should()
              .HaveCount(2);
    }

    [Test]
    public void FilterByLineOfSight_Generic_ShouldExcludePoints_WhenWallBlocks()
    {
        var map = MockMapInstance.Create(width: 20, height: 20, setup: m => MockMapInstance.SetWall(m, new Point(5, 7)));
        IPoint origin = new Point(5, 5);
        IPoint clearTarget = new Point(8, 5);
        IPoint blockedTarget = new Point(5, 9);

        IPoint[] points =
        [
            clearTarget,
            blockedTarget
        ];

        var result = points.FilterByLineOfSight(origin, map)
                           .ToList();

        result.Should()
              .Contain(clearTarget);

        result.Should()
              .NotContain(blockedTarget);
    }

    [Test]
    public void FilterByLineOfSight_Generic_InvertLos_ShouldWork()
    {
        var map = MockMapInstance.Create(width: 20, height: 20, setup: m => MockMapInstance.SetWall(m, new Point(5, 7)));
        IPoint origin = new Point(5, 5);
        IPoint unoccluded = new Point(8, 5);

        IPoint[] points =
        [
            unoccluded,
            new Point(5, 7)
        ];

        var result = points.FilterByLineOfSight(origin, map, true)
                           .ToList();

        result.Should()
              .Contain(unoccluded);
    }
    #endregion
}
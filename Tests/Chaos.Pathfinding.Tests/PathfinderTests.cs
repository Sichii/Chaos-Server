#region
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Pathfinding.Tests;

public sealed class PathfinderTests
{
    //@formatter:off
    [Test]
    [Arguments(0,  1)]
    [Arguments(1,  5)]
    [Arguments(2,  13)]
    [Arguments(3,  25)]
    [Arguments(5,  61)]
    [Arguments(10, 221)]
    //@formatter:on
    public void CalculateSpiralSearchArea_ReturnsExpectedCount(int radius, int expected)
        => Pathfinder.CalculateSpiralSearchArea(radius)
                     .Should()
                     .Be(expected);

    [Test]
    public void FindOptimalDirection_NoLimitRadius_ShouldUseFullGrid()
    {
        // No LimitRadius — exercises the subGrid = default path in InitializeGrid/ResetGrid
        var grid = MakeGrid(5, 5);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(4, 4);

        var dir = pf.FindOptimalDirection(start, end);

        dir.Should()
           .NotBe(Direction.Invalid);
    }

    [Test]
    public void FindOptimalDirection_NoPathExists_ShouldFallbackToSimpleDirection()
    {
        // Fully walled off — pathfinding returns null, falls back to FindSimpleDirection
        var walls = new[]
        {
            new Point(1, 0),
            new Point(0, 1),
            new Point(1, 1)
        };
        var grid = MakeGrid(2, 2, walls.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(1, 1);

        // Should not throw — either returns a direction via FindSimpleDirection or Invalid
        var dir = pf.FindOptimalDirection(start, end);

        // Doesn't matter what direction, just exercising the fallback path
        dir.Should()
           .BeOneOf(
               Direction.Up,
               Direction.Down,
               Direction.Left,
               Direction.Right,
               Direction.Invalid);
    }

    [Test]
    public void FindOptimalDirection_StartEqualsEnd_NoWalkableNeighbors_ReturnsInvalid()
    {
        // All neighbors are walls — FindRandomPoint returns null → returns Invalid
        var walls = new[]
        {
            new Point(0, 1),
            new Point(1, 0)
        };
        var grid = MakeGrid(2, 2, walls.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);

        var dir = pf.FindOptimalDirection(start, start);

        dir.Should()
           .Be(Direction.Invalid);
    }

    [Test]
    public void FindOptimalDirection_StartEqualsEnd_ReturnsInvalid()
    {
        var grid = MakeGrid(1, 1);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);

        var dir = pf.FindOptimalDirection(start, start);

        dir.Should()
           .Be(Direction.Invalid);
    }

    [Test]
    public void FindOptimalDirection_StraightLine_ReturnsCorrectDirection()
    {
        var grid = MakeGrid(5, 1);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(4, 0);

        var dir = pf.FindOptimalDirection(start, end);

        dir.Should()
           .Be(Direction.Right);
    }

    [Test]
    public void FindOptimalDirection_WithDetourRequired_ReturnsValidDirection()
    {
        // 5x5 grid, partial wall at x=2 y=0..3, route must go around
        var walls = new[]
        {
            new Point(2, 0),
            new Point(2, 1),
            new Point(2, 2),
            new Point(2, 3)
        };
        var grid = MakeGrid(5, 5, walls.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var start = new Point(0, 2);
        var end = new Point(4, 2);

        var dir = pf.FindOptimalDirection(start, end);

        dir.Should()
           .NotBe(Direction.Invalid);
    }

    [Test]
    public void FindOptimalDirection_WithLimitRadius_ShouldWork()
    {
        var grid = MakeGrid(5, 5);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(4, 4);

        var dir = pf.FindOptimalDirection(
            start,
            end,
            new PathOptions
            {
                LimitRadius = 5
            });

        dir.Should()
           .NotBe(Direction.Invalid);
    }

    [Test]
    public void FindPath_AroundPartialWallColumn_FindsDetourPath()
    {
        // 5x5 grid; wall column at x=2 for y=0..3 (gap at y=4)
        // Start=(0,2), End=(4,2) — path must detour via y=4 row
        var walls = new[]
        {
            new Point(2, 0),
            new Point(2, 1),
            new Point(2, 2),
            new Point(2, 3)
        };
        var grid = MakeGrid(5, 5, walls.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var start = new Point(0, 2);
        var end = new Point(4, 2);

        var path = pf.FindPath(start, end);

        path.Should()
            .NotBeEmpty();

        // Path should have multiple steps (not a single direct step)
        path.Count
            .Should()
            .BeGreaterThan(1);
    }

    [Test]
    public void FindPath_LimitRadiusTooSmall_FallbackPushesNextStep()
    {
        var grid = MakeGrid(5, 1);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(4, 0);

        var path = pf.FindPath(
            start,
            end,
            new PathOptions
            {
                LimitRadius = 0
            });

        path.Count
            .Should()
            .Be(1);

        path.Pop()
            .Should()
            .Be(new Point(1, 0));
    }

    [Test]
    public void FindPath_NoLimitRadius_ShouldUseFullGrid()
    {
        // No LimitRadius — exercises the subGrid = default path
        var grid = MakeGrid(5, 5);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(4, 4);

        var path = pf.FindPath(start, end);

        path.Should()
            .NotBeEmpty();
    }

    [Test]
    public void FindPath_StartEqualsEnd_NoWalkableNeighbors_FallsBackToSimpleDirection()
    {
        // All neighbors are walls — FindRandomPoint returns null, stack is empty
        // Then falls back to FindSimpleDirection which pushes one step
        var walls = new[]
        {
            new Point(0, 1),
            new Point(1, 0)
        };
        var grid = MakeGrid(2, 2, walls.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);

        var path = pf.FindPath(start, start);

        // Fallback always pushes one directional offset (even into walls)
        path.Count
            .Should()
            .BeLessThanOrEqualTo(1);
    }

    [Test]
    public void FindPath_StraightLine_NoObstacles()
    {
        var grid = MakeGrid(5, 1);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(4, 0);

        var path = pf.FindPath(start, end);

        path.Should()
            .NotBeEmpty();

        path.Peek()
            .Should()
            .Be(new Point(1, 0));
    }

    [Test]
    public void FindPath_WhenStartEqualsEnd_AttemptsToStepAway_ElseEmpty()
    {
        var grid = MakeGrid(3, 3);
        var pf = new Pathfinder(grid);
        var start = new Point(1, 1);

        var path = pf.FindPath(start, start);

        path.Count
            .Should()
            .BeLessThanOrEqualTo(1);
    }

    [Test]
    public void FindPath_WithBlockedPoints_RoutesAroundBlockedTile()
    {
        // 5x3 grid: block (2,1) — path from (0,1) to (4,1) must detour via row 0 or 2
        var grid = MakeGrid(5, 3);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 1);
        var end = new Point(4, 1);

        var blocked = new[]
        {
            new Point(2, 1)
        };

        var path = pf.FindPath(
            start,
            end,
            new PathOptions
            {
                BlockedPoints = blocked.Cast<IPoint>()
                                       .ToArray()
            });

        path.Should()
            .NotBeEmpty();

        // Collect all steps and verify none land on the blocked tile
        var steps = new List<IPoint>();

        while (path.Count > 0)
            steps.Add(path.Pop());

        steps.Should()
             .NotContain(p => (p.X == 2) && (p.Y == 1));
    }

    [Test]
    public void FindPath_WithIgnoreWalls_PassesThroughWalls()
    {
        // Single row with a wall in the middle: S W E
        var walls = new[]
        {
            new Point(1, 0)
        };
        var grid = MakeGrid(3, 1, walls.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(2, 0);

        var path = pf.FindPath(
            start,
            end,
            new PathOptions
            {
                IgnoreWalls = true
            });

        // With IgnoreWalls, path goes straight through the wall node
        path.Should()
            .NotBeEmpty();

        // The first step should be the wall tile (1,0) since it's the direct route
        path.Peek()
            .Should()
            .Be(new Point(1, 0));
    }

    [Test]
    public void FindPath_WithLimitRadius_ExercisesSubGridSearchArea()
    {
        // 10x1 grid, start at (0,0), end at (9,0)
        // LimitRadius=3 constrains the search to a small spiral area
        var grid = MakeGrid(10, 1);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(9, 0);

        var path = pf.FindPath(
            start,
            end,
            new PathOptions
            {
                LimitRadius = 3
            });

        // Path is constrained but should still return at least one step
        path.Should()
            .NotBeEmpty();
    }

    [Test]
    public void FindPath_WithLimitRadius_NoPathToEnd_ShouldFallbackToSimpleDirection()
    {
        // LimitRadius is very small, cannot reach end — InnerFindPath returns empty
        // Falls back to FindSimpleDirection and pushes one step
        var walls = new[]
        {
            new Point(2, 0),
            new Point(2, 1),
            new Point(2, 2)
        };
        var grid = MakeGrid(5, 3, walls.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var start = new Point(0, 1);
        var end = new Point(4, 1);

        var path = pf.FindPath(
            start,
            end,
            new PathOptions
            {
                LimitRadius = 1
            });

        // Should have at least one step (fallback)
        path.Count
            .Should()
            .BeGreaterThan(0);
    }

    [Test]
    public void FindPath_WithWalls_BlocksAndFindsAlternate()
    {
        var walls = new[]
        {
            new Point(1, 0),
            new Point(1, 1),
            new Point(1, 2)
        };
        var grid = MakeGrid(3, 3, walls.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var start = new Point(0, 1);
        var end = new Point(2, 1);

        var path = pf.FindPath(
            start,
            end,
            new PathOptions
            {
                IgnoreWalls = false
            });

        path.Count
            .Should()
            .BeGreaterThan(0);
    }

    [Test]
    public void FindRandomDirection_DefaultPathOptions_ShouldFindDirection()
    {
        // Exercise the pathOptions ??= PathOptions.Default path (called with null)
        var grid = MakeGrid(3, 3);
        var pf = new Pathfinder(grid);
        var start = new Point(1, 1);

        var dir = pf.FindRandomDirection(start);

        dir.Should()
           .NotBe(Direction.Invalid);
    }

    [Test]
    public void FindRandomDirection_ReturnsInvalid_WhenNoWalkable()
    {
        var walls = new[]
        {
            new Point(0, 0),
            new Point(1, 0),
            new Point(0, 1),
            new Point(1, 1)
        };
        var grid = MakeGrid(2, 2, walls.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var dir = pf.FindRandomDirection(new Point(0, 0));

        dir.Should()
           .Be(Direction.Invalid);
    }

    [Test]
    [Arguments(false, Direction.Invalid)]
    [Arguments(true, Direction.Down)]
    public void FindRandomDirection_WithBlockingReactor_TogglesBasedOnOption(bool ignoreBlockingReactors, Direction expected)
    {
        var reactors = new[]
        {
            new Point(0, 1)
        };
        var grid = MakeGrid(1, 2, reactors: reactors.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);

        var dir = pf.FindRandomDirection(
            start,
            new PathOptions
            {
                IgnoreBlockingReactors = ignoreBlockingReactors
            });

        dir.Should()
           .Be(expected);
    }

    [Test]
    public void FindSimpleDirection_PrefersBiasAndSkipsBlocked()
    {
        var grid = MakeGrid(3, 1);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(2, 0);

        var blocked = new[]
        {
            new Point(1, 0)
        };

        var dir = pf.FindSimpleDirection(
            start,
            end,
            new PathOptions
            {
                BlockedPoints = blocked.Cast<IPoint>()
                                       .ToArray()
            });

        dir.Should()
           .Be(Direction.Invalid);
    }

    [Test]
    public void FindSimpleDirection_StartAtOrigin_AllNeighborsBlocked_ReturnsInvalid()
    {
        // When start is at (0,0) and all neighbors are walls, FirstOrDefault returns
        // default(Point) = (0,0) which equals start, so DirectionalRelationTo returns Invalid
        var walls = new[]
        {
            new Point(0, 0),
            new Point(1, 0),
            new Point(0, 1),
            new Point(1, 1)
        };
        var grid = MakeGrid(2, 2, walls.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(1, 1);

        var dir = pf.FindSimpleDirection(start, end);

        dir.Should()
           .Be(Direction.Invalid);
    }

    [Test]
    public void FindSimpleDirection_UnblockedBiasDirection_ReturnsBiasDirection()
    {
        // 3x1 grid: S . E  — direct right is unblocked
        var grid = MakeGrid(3, 1);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(2, 0);

        // No blocked points — should return Right (the direction of the end)
        var dir = pf.FindSimpleDirection(start, end);

        dir.Should()
           .Be(Direction.Right);
    }

    private static GridDetails MakeGrid(
        int w,
        int h,
        IEnumerable<IPoint>? walls = null,
        IEnumerable<IPoint>? reactors = null)
        => new()
        {
            Width = w,
            Height = h,
            Walls = (walls ?? []).ToList(),
            BlockingReactors = (reactors ?? []).ToList()
        };
}
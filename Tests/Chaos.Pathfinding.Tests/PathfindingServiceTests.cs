#region
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
#endregion

namespace Chaos.Pathfinding.Tests;

public sealed class PathfindingServiceTests
{
    [Test]
    public void FindOptimalDirection_DelegatesToPathfinder()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var svc = new PathfindingService(cache);

        svc.RegisterGrid(
            "opt",
            new GridDetails
            {
                Width = 5,
                Height = 1,
                Walls = Array.Empty<IPoint>(),
                BlockingReactors = Array.Empty<IPoint>()
            });

        var dir = svc.FindOptimalDirection("opt", new Point(0, 0), new Point(4, 0));

        dir.Should()
           .Be(Direction.Right);
    }

    [Test]
    public void FindRandomDirection_ReturnsInvalid_WhenNoWalkable()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var svc = new PathfindingService(cache);

        var grid = new GridDetails
        {
            Width = 1,
            Height = 1,
            Walls = [new Point(0, 0)]
        };
        svc.RegisterGrid("g2", grid);

        var dir = svc.FindRandomDirection("g2", new Point(0, 0));

        dir.Should()
           .Be(Direction.Invalid);
    }

    [Test]
    public void FindRandomDirection_ReturnsValidDirection_ForOpenGrid()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var svc = new PathfindingService(cache);

        svc.RegisterGrid(
            "open",
            new GridDetails
            {
                Width = 3,
                Height = 3,
                Walls = Array.Empty<IPoint>(),
                BlockingReactors = Array.Empty<IPoint>()
            });

        var dir = svc.FindRandomDirection("open", new Point(1, 1));

        dir.Should()
           .NotBe(Direction.Invalid);
    }

    [Test]
    public void FindSimpleDirection_ReturnsInvalid_WhenBlocked()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var svc = new PathfindingService(cache);

        var grid = new GridDetails
        {
            Width = 3,
            Height = 1,
            Walls = Array.Empty<IPoint>(),
            BlockingReactors = Array.Empty<IPoint>()
        };
        svc.RegisterGrid("g3", grid);

        var dir = svc.FindSimpleDirection(
            "g3",
            new Point(0, 0),
            new Point(2, 0),
            new PathOptions
            {
                BlockedPoints = [new Point(1, 0)]
            });

        dir.Should()
           .Be(Direction.Invalid);
    }

    [Test]
    public void MissingGrid_Throws_KeyNotFound()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var svc = new PathfindingService(cache);

        Action act = () => svc.FindPath("missing", new Point(0, 0), new Point(1, 0));

        act.Should()
           .Throw<KeyNotFoundException>();
    }

    [Test]
    public void RegisterGrid_And_FindPath_UsesCachedPathfinder()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var svc = new PathfindingService(cache);

        var grid = new GridDetails
        {
            Width = 3,
            Height = 1,
            Walls = Array.Empty<IPoint>(),
            BlockingReactors = Array.Empty<IPoint>()
        };
        svc.RegisterGrid("g", grid);

        var start = new Point(0, 0);
        var end = new Point(2, 0);

        var path1 = svc.FindPath("g", start, end);
        var path2 = svc.FindPath("g", start, end);

        path1.Should()
             .NotBeEmpty();

        path2.Should()
             .NotBeEmpty();
    }

    [Test]
    public void RegisterGrid_KeyIsCaseInsensitive()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var svc = new PathfindingService(cache);

        svc.RegisterGrid(
            "CaseKey",
            new GridDetails
            {
                Width = 2,
                Height = 1
            });

        var path = svc.FindPath("casekey", new Point(0, 0), new Point(1, 0));

        path.Should()
            .NotBeNull();
    }

    [Test]
    public void RegisterGrid_OverwritesSameKey()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var svc = new PathfindingService(cache);

        var grid1 = new GridDetails
        {
            Width = 1,
            Height = 1,
            Walls = [new Point(0, 0)]
        };

        var grid2 = new GridDetails
        {
            Width = 3,
            Height = 1,
            Walls = Array.Empty<IPoint>(),
            BlockingReactors = Array.Empty<IPoint>()
        };

        svc.RegisterGrid("overwrite", grid1);
        svc.RegisterGrid("overwrite", grid2);

        var path = svc.FindPath("overwrite", new Point(0, 0), new Point(2, 0));

        path.Should()
            .NotBeEmpty();
    }
}
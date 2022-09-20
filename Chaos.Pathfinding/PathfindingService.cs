using System.Collections.Concurrent;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Definitions;
using Chaos.Pathfinding.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Pathfinding;

public class PathfindingService : IPathfindingService
{
    private readonly ConcurrentDictionary<string, IGridDetails> GridDetails;
    private readonly IMemoryCache MemoryCache;

    public PathfindingService(IMemoryCache memoryCache)
    {
        GridDetails = new ConcurrentDictionary<string, IGridDetails>(StringComparer.OrdinalIgnoreCase);
        MemoryCache = memoryCache;
    }

    private IPathfinder CreatePathfinder(ICacheEntry cacheEntry)
    {
        cacheEntry.SetSlidingExpiration(TimeSpan.FromMinutes(5));

        var key = cacheEntry.Key.ToString();

        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("Key cannot be null or empty");

        if (!GridDetails.TryGetValue(key, out var gridDetails))
            throw new KeyNotFoundException($"{key} not found for pathfinder grid details");

        return new Pathfinder(gridDetails);
    }

    public Direction Pathfind(
        string key,
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        ICollection<IPoint> creatures
    )
    {
        var pathFinder = MemoryCache.GetOrCreate(key, CreatePathfinder);

        return pathFinder!.Pathfind(
            start,
            end,
            ignoreWalls,
            creatures);
    }

    public void RegisterGrid(string key, IGridDetails gridDetails) => GridDetails[key] = gridDetails;

    /// <inheritdoc />
    public Direction Wander(
        string key,
        IPoint start,
        bool ignoreWalls,
        ICollection<IPoint> creatures
    )
    {
        var pathFinder = MemoryCache.GetOrCreate(key, CreatePathfinder);

        return pathFinder!.Wander(start, ignoreWalls, creatures);
    }
}
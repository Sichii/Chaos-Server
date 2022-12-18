using System.Collections.Concurrent;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Pathfinding.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Pathfinding;

public sealed class PathfindingService : IPathfindingService
{
    private const string KEY_PREFIX = $"{nameof(PathfindingService)}___";
    private readonly ConcurrentDictionary<string, IGridDetails> GridDetails;
    private readonly IMemoryCache MemoryCache;

    public PathfindingService(IMemoryCache memoryCache)
    {
        GridDetails = new ConcurrentDictionary<string, IGridDetails>(StringComparer.OrdinalIgnoreCase);
        MemoryCache = memoryCache;
    }

    private string ConstructKey(string key) => $"{KEY_PREFIX}{key}".ToLowerInvariant();

    private IPathfinder CreatePathfinder(ICacheEntry cacheEntry)
    {
        cacheEntry.SetSlidingExpiration(TimeSpan.FromMinutes(60));

        var key = cacheEntry.Key.ToString();

        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("Key cannot be null or empty");

        var keyActual = DeconstructKey(key!);

        if (!GridDetails.TryGetValue(keyActual, out var gridDetails))
            throw new KeyNotFoundException($"{keyActual} not found for pathfinder grid details");

        return new Pathfinder(gridDetails);
    }

    private string DeconstructKey(string key) => key.Replace(KEY_PREFIX, string.Empty, StringComparison.OrdinalIgnoreCase);

    public Direction Pathfind(
        string key,
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        ICollection<IPoint> unwalkablePoints
    )
    {
        var lookupKey = ConstructKey(key);

        var pathFinder = MemoryCache.GetOrCreate(lookupKey, CreatePathfinder);

        return pathFinder!.Pathfind(
            start,
            end,
            ignoreWalls,
            unwalkablePoints);
    }

    public void RegisterGrid(string key, IGridDetails gridDetails) => GridDetails[key.ToLowerInvariant()] = gridDetails;

    /// <inheritdoc />
    public Direction Wander(
        string key,
        IPoint start,
        bool ignoreWalls,
        ICollection<IPoint> unwalkablePoints
    )
    {
        var lookupKey = ConstructKey(key);

        var pathFinder = MemoryCache.GetOrCreate(lookupKey, CreatePathfinder);

        return pathFinder!.Wander(start, ignoreWalls, unwalkablePoints);
    }
}
using System.Collections.Concurrent;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Pathfinding.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Pathfinding;

/// <summary>
///     Provides a service for pathfinding
/// </summary>
public sealed class PathfindingService : IPathfindingService
{
    private const string KEY_PREFIX = $"{nameof(PathfindingService)}___";
    private readonly ConcurrentDictionary<string, IGridDetails> GridDetails;
    private readonly IMemoryCache MemoryCache;

    /// <summary>
    ///     Creates a new instance of <see cref="PathfindingService" />
    /// </summary>
    /// <param name="memoryCache">A cache to store pathfinding grids in</param>
    public PathfindingService(IMemoryCache memoryCache)
    {
        GridDetails = new ConcurrentDictionary<string, IGridDetails>(StringComparer.OrdinalIgnoreCase);
        MemoryCache = memoryCache;
    }

    /// <summary>
    ///     Finds a path between two points
    /// </summary>
    /// <param name="gridKey">The key of the grid to find a path on</param>
    /// <param name="start">The starting point</param>
    /// <param name="end">The ending point</param>
    /// <param name="ignoreWalls">Whether or not to ignore walls</param>
    /// <param name="blocked">A collection of extra unwalkable points such as creatures</param>
    /// <param name="limitRadius">
    ///     Specify a max radius to use for path calculation, this can help with performance by limiting
    ///     node discovery
    /// </param>
    /// <returns></returns>
    public Direction Pathfind(
        string gridKey,
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        IReadOnlyCollection<IPoint> blocked,
        int? limitRadius = null
    )
    {
        var lookupKey = ConstructKey(gridKey);

        var pathFinder = MemoryCache.GetOrCreate(lookupKey, CreatePathfinder);

        return pathFinder!.Pathfind(
            start,
            end,
            ignoreWalls,
            blocked,
            limitRadius);
    }

    /// <summary>
    ///     Registers a grid to be used for pathfinding
    /// </summary>
    /// <param name="key">The key used to look up the grid</param>
    /// <param name="gridDetails">Details used to pathfind on the grid</param>
    public void RegisterGrid(string key, IGridDetails gridDetails) => GridDetails[key] = gridDetails;

    /// <inheritdoc />
    public Direction Wander(
        string key,
        IPoint start,
        bool ignoreWalls,
        IReadOnlyCollection<IPoint> blocked
    )
    {
        var lookupKey = ConstructKey(key);

        //not thread safe, but it should be fine if we occasionally create a duplicate pathfinder
        var pathFinder = MemoryCache.GetOrCreate(lookupKey, CreatePathfinder);

        return pathFinder!.Wander(start, ignoreWalls, blocked);
    }

    private string ConstructKey(string key) => $"{KEY_PREFIX}{key}".ToLowerInvariant();

    private IPathfinder CreatePathfinder(ICacheEntry cacheEntry)
    {
        cacheEntry.SetSlidingExpiration(TimeSpan.FromMinutes(60));

        var key = cacheEntry.Key.ToString();

        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("Key cannot be null or empty");

        var keyActual = DeconstructKey(key);

        if (!GridDetails.TryGetValue(keyActual, out var gridDetails))
            throw new KeyNotFoundException($"{keyActual} not found for pathfinder grid details");

        return new Pathfinder(gridDetails);
    }

    private string DeconstructKey(string key) => key.Replace(KEY_PREFIX, string.Empty, StringComparison.OrdinalIgnoreCase);
}
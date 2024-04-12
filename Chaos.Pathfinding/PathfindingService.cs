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
    /// <param name="memoryCache">
    ///     A cache to store pathfinding grids in
    /// </param>
    public PathfindingService(IMemoryCache memoryCache)
    {
        GridDetails = new ConcurrentDictionary<string, IGridDetails>(StringComparer.OrdinalIgnoreCase);
        MemoryCache = memoryCache;
    }

    /// <inheritdoc />
    public Stack<IPoint> FindPath(
        string gridKey,
        IPoint start,
        IPoint end,
        IPathOptions? pathOptions = null)
    {
        var lookupKey = ConstructKey(gridKey);

        var pathFinder = MemoryCache.GetOrCreate(lookupKey, CreatePathfinder);

        return pathFinder!.FindPath(start, end, pathOptions);
    }

    /// <inheritdoc />
    public Direction FindRandomDirection(string key, IPoint start, IPathOptions? pathOptions = null)
    {
        var lookupKey = ConstructKey(key);

        //not thread safe, but it should be fine if we occasionally create a duplicate pathfinder
        var pathFinder = MemoryCache.GetOrCreate(lookupKey, CreatePathfinder);

        return pathFinder!.FindRandomDirection(start, pathOptions);
    }

    /// <inheritdoc />
    public Direction FindSimpleDirection(
        string gridKey,
        IPoint start,
        IPoint end,
        IPathOptions? pathOptions = null)
    {
        var lookupKey = ConstructKey(gridKey);

        var pathFinder = MemoryCache.GetOrCreate(lookupKey, CreatePathfinder);

        return pathFinder!.FindSimpleDirection(start, end, pathOptions);
    }

    /// <inheritdoc />
    public void RegisterGrid(string key, IGridDetails gridDetails) => GridDetails[key] = gridDetails;

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
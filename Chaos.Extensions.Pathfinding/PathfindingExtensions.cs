using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.Pathfinding"/> DI extensions
/// </summary>
public static class PathfindingExtensions
{
    /// <summary>
    ///     Adds <see cref="PathfindingService"/> as an implementation of <see cref="IPathfindingService"/> to the service collection
    /// </summary>
    /// <param name="serviceCollection">The service collection to add to</param>
    /// <remarks>Depends on <see cref="IMemoryCache"/></remarks>
    public static void AddPathfinding(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMemoryCache();
        serviceCollection.AddSingleton<IPathfindingService, PathfindingService>();
    }
}
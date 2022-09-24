using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

public static class PathfindingExtensions
{
    public static void AddPathfinding(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMemoryCache();
        serviceCollection.AddSingleton<IPathfindingService, PathfindingService>();
    }
}
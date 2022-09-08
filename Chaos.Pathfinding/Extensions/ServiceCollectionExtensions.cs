using Chaos.Pathfinding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Pathfinding.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPathfinding(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMemoryCache();
        serviceCollection.AddSingleton<IPathfindingService, PathfindingService>();
    }
}
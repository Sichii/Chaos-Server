using Chaos.Pathfinding.Interfaces;
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
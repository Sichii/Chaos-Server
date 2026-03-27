#region
using Chaos.Extensions.DependencyInjection;
using Chaos.Pathfinding.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Chaos.Pathfinding.Tests;

public sealed class PathfindingExtensionsTests
{
    [Test]
    public void AddPathfinding_Registers_Service_And_Cache()
    {
        var services = new ServiceCollection();
        services.AddPathfinding();
        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IPathfindingService>()
                .Should()
                .NotBeNull();
    }
}
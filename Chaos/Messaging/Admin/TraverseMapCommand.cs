#region
using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Services.Other.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Messaging.Admin;

[Command("traverse", helpText: "<mapInstanceId> <xPos?> <yPos?>")]
public sealed class TraverseMapCommand : ICommand<Aisling>
{
    private readonly ISimpleCache Cache;
    private readonly IMapTraversalService MapTraversalService;

    public TraverseMapCommand(ISimpleCache cache, IMapTraversalService mapTraversalService)
    {
        Cache = cache;
        MapTraversalService = mapTraversalService;
    }

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling aisling, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var mapInstanceId))
            return default;

        var mapInstance = Cache.Get<MapInstance>(mapInstanceId);

        Point point;

        if (args.TryGetNext<int>(out var xPos) && args.TryGetNext<int>(out var yPos))
            point = new Point(xPos, yPos);
        else
            point = new Point(mapInstance.Template.Width / 2, mapInstance.Template.Height / 2);

        MapTraversalService.AdminTraverseMap(aisling, mapInstance, point);

        return default;
    }
}
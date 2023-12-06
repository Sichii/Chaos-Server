using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Storage.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("traverse", helpText: "<mapInstanceId> <xPos?> <yPos?>")]
public sealed class TraverseMapCommand(ISimpleCache cache) : ICommand<Aisling>
{
    private readonly ISimpleCache Cache = cache;

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

        aisling.TraverseMap(mapInstance, point, true);

        return default;
    }
}
using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Storage.Abstractions;

namespace Chaos.Commands;

[Command("tp")]
public sealed class TeleportCommand : ICommand<Aisling>
{
    private readonly ISimpleCache Cache;
    public TeleportCommand(ISimpleCache cache) => Cache = cache;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling aisling, ArgumentCollection args)
    {
        if (!args.TryGet<string>(0, out var mapInstanceId))
            return default;

        var mapInstance = Cache.Get<MapInstance>(mapInstanceId);
        Point point;

        if (args.TryGet<int>(1, out var xPos) && args.TryGet<int>(2, out var yPos))
            point = new Point(xPos, yPos);
        else
            point = new Point(mapInstance.Template.Width / 2, mapInstance.Template.Height / 2);

        aisling.TraverseMap(mapInstance, point);

        return default;
    }
}
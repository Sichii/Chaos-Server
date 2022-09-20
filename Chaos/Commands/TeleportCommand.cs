using Chaos.Commands.Abstractions;
using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Services.Caches.Abstractions;

namespace Chaos.Commands;

[Command("tp")]
public class TeleportCommand : ICommand
{
    private readonly ISimpleCache Cache;
    public TeleportCommand(ISimpleCache cache) => Cache = cache;

    /// <inheritdoc />
    public void Execute(Aisling aisling, params string[] args)
    {
        switch (args.Length)
        {
            case 1:
            {
                var mapInstanceId = args[0];
                var mapInstance = Cache.GetObject<MapInstance>(mapInstanceId);
                var centerPoint = new Point(mapInstance.Template.Width / 2, mapInstance.Template.Height / 2);
                aisling.TraverseMap(mapInstance, centerPoint);

                break;
            }
            case 3:
            {
                var mapInstanceId = args[0];
                var x = int.Parse(args[1]);
                var y = int.Parse(args[2]);
                var point = new Point(x, y);
                var mapInstance = Cache.GetObject<MapInstance>(mapInstanceId);
                aisling.TraverseMap(mapInstance, point);

                break;
            }
        }
    }
}
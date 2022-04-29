using Chaos.Containers;
using Chaos.DataObjects;
using Chaos.Managers.Interfaces;
using Chaos.WorldObjects.Abstractions;

namespace Chaos.WorldObjects;

public class WarpTile : ReactorTile
{
    private readonly ICacheManager<string, MapInstance> MapInstanceManager;
    public Warp Warp { get; init; }

    public WarpTile(ICacheManager<string, MapInstance> mapInstanceManager, Warp warp)
        : base(mapInstanceManager.GetObject(warp.SourceLocation!.Value.MapId), warp.SourceLocation!.Value.Point)
    {
        MapInstanceManager = mapInstanceManager;
        Warp = warp;
    }

    public override void Activate(Creature creature)
    {
        if (creature is User user)
        {
            var targetMap = MapInstanceManager.GetObject(Warp.TargetLocation.MapId);
            MapInstance.RemoveObject(user);
            targetMap.AddObject(user, Warp.TargetLocation.Point);
        }
    }
}
using Chaos.Caches.Interfaces;
using Chaos.Containers;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

public class WarpTile : ReactorTile
{
    private readonly ISimpleCache<string, MapInstance> MapInstanceCache;
    public Warp Warp { get; init; }

    public WarpTile(ISimpleCache<string, MapInstance> mapInstanceCache, Warp warp)
        : base(mapInstanceCache.GetObject(warp.SourceLocation!.Value.MapId), warp.SourceLocation!.Value.Point)
    {
        MapInstanceCache = mapInstanceCache;
        Warp = warp;
    }

    public override void Activate(Creature creature)
    {
        if (creature is User user)
        {
            var targetMap = MapInstanceCache.GetObject(Warp.TargetLocation.MapId);
            this.MapInstance.RemoveObject(user);
            targetMap.AddObject(user, Warp.TargetLocation.Point);
        }
    }
}
using Chaos.Caches.Interfaces;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

public class WarpTile : ReactorTile
{
    private readonly ISimpleCache<MapInstance> MapInstanceCache;
    public Warp Warp { get; }

    public override ReactorTileType ReactorTileType => ReactorTileType.Walk;

    public override void Activate(Creature creature)
    {
        var targetMap = MapInstanceCache.GetObject(Warp.TargetLocation.Map);
        creature.TraverseMap(targetMap, Warp.TargetLocation);
    }

    public WarpTile(Warp warp, ISimpleCache<MapInstance> mapInstanceCache)
        : base(mapInstanceCache.GetObject(warp.SourceLocation!.Value.Map), warp.SourceLocation.Value)
    {
        Warp = warp;
        MapInstanceCache = mapInstanceCache;
    }
}
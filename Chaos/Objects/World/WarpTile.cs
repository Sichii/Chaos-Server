using Chaos.Containers;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Objects.World.Abstractions;
using Chaos.Services.Caches.Interfaces;

namespace Chaos.Objects.World;

public class WarpTile : ReactorTile
{
    private readonly ISimpleCache<MapInstance> MapInstanceCache;

    public override ReactorTileType ReactorTileType => ReactorTileType.Walk;
    public Warp Warp { get; }

    public WarpTile(Warp warp, ISimpleCache<MapInstance> mapInstanceCache)
        : base(mapInstanceCache.GetObject(warp.SourceLocation!.Value.Map), warp.SourceLocation.Value)
    {
        Warp = warp;
        MapInstanceCache = mapInstanceCache;
    }

    public override void Activate(Creature creature)
    {
        var targetMap = MapInstanceCache.GetObject(Warp.TargetLocation.Map);
        creature.TraverseMap(targetMap, Warp.TargetLocation);
    }
}
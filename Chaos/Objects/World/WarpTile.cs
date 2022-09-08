using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Objects.World.Abstractions;
using Chaos.Services.Caches.Abstractions;

namespace Chaos.Objects.World;

public class WarpTile : ReactorTile
{
    private readonly ISimpleCache SimpleCache;

    public override ReactorTileType ReactorTileType => ReactorTileType.Walk;
    public Warp Warp { get; }

    public WarpTile(Warp warp, ISimpleCache simpleCache)
        : base(simpleCache.GetObject<MapInstance>(warp.SourceLocation!.Value.Map), warp.SourceLocation.Value)
    {
        Warp = warp;
        SimpleCache = simpleCache;
    }

    public override void Activate(Creature creature)
    {
        var targetMap = SimpleCache.GetObject<MapInstance>(Warp.TargetLocation.Map);
        creature.TraverseMap(targetMap, Warp.TargetLocation);
    }
}
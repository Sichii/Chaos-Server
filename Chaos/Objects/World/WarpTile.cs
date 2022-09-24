using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Objects.World;

public sealed class WarpTile : ReactorTile
{
    private readonly ISimpleCache SimpleCache;

    public override ReactorTileType ReactorTileType => ReactorTileType.Walk;
    public Warp Warp { get; }

    public WarpTile(MapInstance mapInstance, ISimpleCache simpleCache, Warp warp)
        : base(mapInstance, warp.Source ?? throw new ArgumentNullException(nameof(warp.Source)))
    {
        Warp = warp;
        SimpleCache = simpleCache;
    }

    public override void Activate(Creature creature)
    {
        var targetMap = SimpleCache.GetObject<MapInstance>(Warp.Destination.Map);
        var aisling = creature as Aisling;

        if (creature.StatSheet.Level < (targetMap.MinimumLevel ?? 0))
        {
            aisling?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"You must be at least level {targetMap.MinimumLevel} to enter this area.");

            var point = creature.DirectionalOffset(creature.Direction.Reverse());
            creature.WarpTo(point);

            return;
        }

        if (creature.StatSheet.Level > (targetMap.MaximumLevel ?? int.MaxValue))
        {
            aisling?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"You must be at most level {targetMap.MaximumLevel} to enter this area.");

            var point = creature.DirectionalOffset(creature.Direction.Reverse());
            creature.WarpTo(point);

            return;
        }

        creature.TraverseMap(targetMap, Warp.Destination);
    }
}
using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.ReactorTileScripts;

public class WarpScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;
    protected Location Destination { get; init; }
    
    /// <inheritdoc />
    public WarpScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        var targetMap = SimpleCache.Get<MapInstance>(Destination.Map);
        var aisling = source as Aisling;

        if (source.StatSheet.Level < (targetMap.MinimumLevel ?? 0))
        {
            aisling?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"You must be at least level {targetMap.MinimumLevel} to enter this area.");

            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        if (source.StatSheet.Level > (targetMap.MaximumLevel ?? int.MaxValue))
        {
            aisling?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"You must be at most level {targetMap.MaximumLevel} to enter this area.");

            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        source.TraverseMap(targetMap, Destination);
    }
}
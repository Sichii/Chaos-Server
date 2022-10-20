using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

public class WorldMapTile : ReactorTile
{
    public WorldMap WorldMap { get; }

    /// <inheritdoc />
    public WorldMapTile(MapInstance mapInstance, IPoint point, WorldMap worldMap)
        : base(mapInstance, point) => WorldMap = worldMap;

    /// <inheritdoc />
    public override ReactorActivationType ReactorActivationType => ReactorActivationType.Walk;

    /// <inheritdoc />
    public override void Activate(Creature creature)
    {
        if (creature is not Aisling aisling)
            return;

        aisling.MapInstance.RemoveObject(creature);
        aisling.Client.SendWorldMap(WorldMap);
    }
}
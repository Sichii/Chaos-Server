using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Geometry.Abstractions;

namespace Chaos.Objects.World.Abstractions;

public abstract class ReactorTile : MapEntity
{
    public abstract ReactorTileType ReactorTileType { get; }

    protected ReactorTile(MapInstance mapInstance, IPoint point)
        : base(mapInstance, point) { }

    public abstract void Activate(Creature creature);
}
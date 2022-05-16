using Chaos.Containers;

namespace Chaos.Objects.World.Abstractions;

public abstract class ReactorTile : MapObject
{
    protected ReactorTile(MapInstance mapInstance, Point point)
        : base(mapInstance, point) { }

    public abstract void Activate(Creature creature);
}
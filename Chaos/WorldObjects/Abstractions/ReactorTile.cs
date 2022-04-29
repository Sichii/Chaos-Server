using Chaos.Containers;
using Chaos.Core.Geometry;
using Chaos.Interfaces;

namespace Chaos.WorldObjects.Abstractions;

public abstract class ReactorTile : MapObject, IReactorTile
{
    protected ReactorTile(MapInstance mapInstance, Point point)
        : base(mapInstance, point) { }

    public abstract void Activate(Creature creature);
}
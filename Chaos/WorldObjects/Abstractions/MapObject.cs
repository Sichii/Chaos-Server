using Chaos.Containers;
using Chaos.Core.Geometry;

namespace Chaos.WorldObjects.Abstractions;

public abstract class MapObject : WorldObject
{
    public MapInstance MapInstance { get; set; }
    public Point Point { get; set; }
    public Location Location => (MapInstance.InstanceId, Point);

    protected internal MapObject(MapInstance mapInstance, Point point)
    {
        MapInstance = mapInstance;
        Point = point;
    }

    public int Distance(MapObject mapObject) => Distance(mapObject.Location);
    public int Distance(Location location) => Location.Distance(location);

    public int Distance(Point point) => Location.Point.Distance(point);

    internal bool WithinRange(MapObject mapObject, int range = 13) => Distance(mapObject) < range;

    internal bool WithinRange(Location location, int range = 13) => Distance(location) < range;

    internal bool WithinRange(Point p, int range = 13) => Location.Point.Distance(p) < range;
}
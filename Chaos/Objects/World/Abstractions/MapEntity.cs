using Chaos.Containers;
using Chaos.Geometry.Interfaces;

namespace Chaos.Objects.World.Abstractions;

public abstract class MapEntity : WorldEntity, ILocation
{
    public MapInstance MapInstance { get; set; } = null!;
    public int X { get; set; }
    public int Y { get; set; }
    
    protected MapEntity(MapInstance mapInstance, IPoint point) => SetLocation(mapInstance, point);

    public void SetLocation(IPoint point)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point));

        X = point.X;
        Y = point.Y;
    }
    
    public void SetLocation(MapInstance mapInstance, IPoint point)
    {
        if (mapInstance == null)
            throw new ArgumentNullException(nameof(mapInstance));

        if (point == null)
            throw new ArgumentNullException(nameof(point));

        SetLocation(point);
        MapInstance = mapInstance;
    }

    public virtual void WarpTo(IPoint destinationPoint) => SetLocation(destinationPoint);

    string ILocation.Map => MapInstance.InstanceId;
}
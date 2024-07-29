using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;

namespace Chaos.Models.World.Abstractions;

public abstract class MapEntity(MapInstance mapInstance, IPoint point) : WorldEntity, ILocation
{
    public MapInstance MapInstance { get; set; } = mapInstance;
    public int X { get; private set; } = point.X;
    public int Y { get; private set; } = point.Y;
    string ILocation.Map => MapInstance.InstanceId;

    public virtual void Animate(Animation animation, uint? sourceId = null)
    {
        var pointAnimation = animation.GetPointAnimation(Point.From(this), sourceId);
        MapInstance.ShowAnimation(pointAnimation);
    }

    public void SetLocation(IPoint point)
    {
        ArgumentNullException.ThrowIfNull(point);

        if (!MapInstance.IsWithinMap(point))
        {
            if (this is Aisling aisling)
                aisling.Refresh(true);

            throw new InvalidOperationException(
                $"Attempted to set location outside of map bounds. (Map: {MapInstance.InstanceId}, Destination: {point})");
        }

        MapInstance.MoveEntity(this, Point.From(point));
    }

    public void SetLocation(MapInstance mapInstance, IPoint point)
    {
        // ReSharper disable once JoinNullCheckWithUsage
        ArgumentNullException.ThrowIfNull(mapInstance);

        ArgumentNullException.ThrowIfNull(point);

        X = point.X;
        Y = point.Y;
        MapInstance = mapInstance;
    }

    public virtual void WarpTo(IPoint destinationPoint) => SetLocation(destinationPoint);
}
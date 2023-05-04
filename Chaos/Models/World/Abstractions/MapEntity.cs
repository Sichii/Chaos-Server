using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;

namespace Chaos.Models.World.Abstractions;

public abstract class MapEntity : WorldEntity, ILocation
{
    public MapInstance MapInstance { get; set; }
    public int X { get; private set; }
    public int Y { get; private set; }

    string ILocation.Map => MapInstance.InstanceId;

    protected MapEntity(MapInstance mapInstance, IPoint point)
    {
        MapInstance = mapInstance;
        X = point.X;
        Y = point.Y;
    }

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

        var oldPoint = Point.From(this);

        X = point.X;
        Y = point.Y;

        MapInstance.MoveEntity(this, oldPoint);

        if (this is Creature creature)
            creature.LastMove = DateTime.UtcNow;
    }

    public void SetLocationFaux(MapInstance mapInstance, IPoint point)
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
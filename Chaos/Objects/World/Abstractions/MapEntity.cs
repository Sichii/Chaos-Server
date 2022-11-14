using Chaos.Containers;
using Chaos.Data;
using Chaos.Geometry.Abstractions;

namespace Chaos.Objects.World.Abstractions;

public abstract class MapEntity : WorldEntity, ILocation
{
    public MapInstance MapInstance { get; set; } = null!;
    public int X { get; private set; }
    public int Y { get; private set; }

    string ILocation.Map => MapInstance.InstanceId;

    protected MapEntity(MapInstance mapInstance, IPoint point) => SetLocation(mapInstance, point);

    public virtual void Animate(Animation animation, uint? sourceId = null)
    {
        var pointAnimation = animation.GetPointAnimation(Point.From(this), sourceId);
        MapInstance.ShowAnimation(pointAnimation);
    }

    public void SetLocation(IPoint point)
    {
        ArgumentNullException.ThrowIfNull(point);

        var oldPoint = Point.From(this);

        X = point.X;
        Y = point.Y;

        if (oldPoint != this)
        {
            MapInstance.MoveEntity(this, oldPoint);

            if (this is Creature creature)
                creature.LastMove = DateTime.UtcNow;
        }
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

    /// <inheritdoc />
    public override string ToString() => $"Id:{Id} Loc:{ILocation.ToString(this)}";

    public virtual void WarpTo(IPoint destinationPoint) => SetLocation(destinationPoint);
}
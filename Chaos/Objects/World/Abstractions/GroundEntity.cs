using Chaos.Containers;
using Chaos.Geometry.Abstractions;

namespace Chaos.Objects.World.Abstractions;

public abstract class GroundEntity : NamedEntity
{
    public TimeSpan TimeOnGround { get; set; }

    /// <inheritdoc />
    protected GroundEntity(
        string name,
        ushort sprite,
        MapInstance mapInstance,
        IPoint point
    )
        : base(
            name,
            sprite,
            mapInstance,
            point) { }

    /// <inheritdoc />
    public override void OnClicked(Aisling source) { }

    public override void Update(TimeSpan delta)
    {
        TimeOnGround += delta;

        //if the entity has been on the ground for over an hour, destroy it
        if (TimeOnGround.TotalHours > 1)
            MapInstance.RemoveObject(this);
    }
}
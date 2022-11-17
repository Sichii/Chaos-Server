using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Objects.World.Abstractions;

public abstract class GroundEntity : NamedEntity
{
    private readonly IIntervalTimer GroundTimer;

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
            point) =>
        GroundTimer = new IntervalTimer(TimeSpan.FromHours(1), false);

    /// <inheritdoc />
    public override void OnClicked(Aisling source) { }

    public override void Update(TimeSpan delta)
    {
        GroundTimer.Update(delta);
        
        //if the entity has been on the ground for over an hour, destroy it
        if (GroundTimer.IntervalElapsed)
            MapInstance.RemoveObject(this);
    }
}
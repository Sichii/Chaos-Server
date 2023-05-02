using Chaos.Containers;
using Chaos.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Objects.World.Abstractions;

public abstract class GroundEntity : NamedEntity
{
    private readonly IIntervalTimer GroundTimer;
    private IIntervalTimer? LockTimer;
    protected HashSet<string>? Owners;

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

    public virtual bool CanPickUp(Aisling source) => true;

    public void LockToCreatures(int seconds, params Aisling[] aislings)
    {
        if (seconds <= 0)
            return;

        LockTimer = new IntervalTimer(TimeSpan.FromSeconds(seconds), false);
        Owners = aislings.Select(aisling => aisling.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override void OnClicked(Aisling source) { }

    /// <inheritdoc />
    public override void SetVisibility(VisibilityType newVisibilityType)
    {
        if (Visibility != newVisibilityType)
        {
            var stack = MapInstance.GetEntitiesAtPoint<GroundEntity>(this)
                                   .OrderBy(entity => entity.Creation)
                                   .ToList();

            foreach (var entity in stack)
                entity.Hide();

            Visibility = newVisibilityType;

            foreach (var entity in stack)
                entity.Display();
        }
    }

    public override void Update(TimeSpan delta)
    {
        GroundTimer.Update(delta);
        LockTimer?.Update(delta);

        if (LockTimer is { IntervalElapsed: true })
        {
            LockTimer = null;
            Owners = null;
        }

        //if the entity has been on the ground for over an hour, destroy it
        if (GroundTimer.IntervalElapsed)
            MapInstance.RemoveObject(this);
    }
}
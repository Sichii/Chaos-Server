using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Models.World.Abstractions;

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
        GroundTimer = new IntervalTimer(TimeSpan.FromMinutes(WorldOptions.Instance.GroundItemDespawnTimeMins), false);

    public virtual bool CanPickUp(Aisling source) => Owners.IsNullOrEmpty() || source.IsAdmin || Owners!.Contains(source.Name);

    public void LockToAislings(int seconds, params Aisling[] aislings)
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

        //if the entity has been on the ground for longer than the configured time, destroy it
        if (GroundTimer.IntervalElapsed)
            MapInstance.RemoveObject(this);
    }
}
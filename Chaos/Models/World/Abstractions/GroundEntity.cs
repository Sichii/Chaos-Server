#region
using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Time;
using Chaos.Time.Abstractions;
#endregion

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
        IPoint point)
        : base(
            name,
            sprite,
            mapInstance,
            point)
        => GroundTimer = new IntervalTimer(TimeSpan.FromMinutes(WorldOptions.Instance.GroundItemDespawnTimeMins), false);

    public virtual bool CanBePickedUp(Aisling source) => Owners.IsNullOrEmpty() || source.IsAdmin || Owners!.Contains(source.Name);

    public void LockToAislings(int seconds, params IEnumerable<Aisling> aislings)
    {
        if (seconds <= 0)
            return;

        LockTimer = new IntervalTimer(TimeSpan.FromSeconds(seconds), false);

        Owners = aislings.Select(aisling => aisling.Name)
                         .ToHashSet(StringComparer.OrdinalIgnoreCase);
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
            MapInstance.RemoveEntity(this);
    }
}
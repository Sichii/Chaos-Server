using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;

namespace Chaos.Models.World.Abstractions;

/// <summary>
///     Represents an object that is visible.
/// </summary>
public abstract class VisibleEntity(ushort sprite, MapInstance mapInstance, IPoint point) : InteractableEntity(mapInstance, point)
{
    protected ConcurrentDictionary<uint, DateTime> LastClicked { get; init; } = new();
    public ushort Sprite { get; set; } = sprite;
    public VisibilityType Visibility { get; protected set; }

    public void Display()
    {
        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                           .ThatCanObserve(this))
            ShowTo(aisling);
    }

    public void Hide()
    {
        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                           .ThatCanObserve(this))
            if (!aisling.Equals(this))
                HideFrom(aisling);
    }

    public virtual void HideFrom(Aisling aisling) => aisling.Client.SendRemoveEntity(Id);

    public virtual void SetVisibility(VisibilityType newVisibilityType)
    {
        if (Visibility != newVisibilityType)
        {
            Visibility = newVisibilityType;
            MapInstance.UpdateNearbyViewPorts(this);

            //update display for all ppl that can still see you
            Display();
        }
    }

    public virtual bool ShouldRegisterClick(uint fromId)
        => !LastClicked.TryGetValue(fromId, out var lastClick)
           || (DateTime.UtcNow.Subtract(lastClick)
                       .TotalMilliseconds
               > 500);

    public virtual void ShowTo(Aisling aisling) => aisling.Client.SendVisibleEntities(this);

    public override void WarpTo(IPoint destinationPoint)
    {
        var startPoint = Point.From(this);
        SetLocation(destinationPoint);

        var creaturesToUpdate = MapInstance.GetEntitiesWithinRange<Creature>(startPoint)
                                           .Union(MapInstance.GetEntitiesWithinRange<Creature>(destinationPoint))
                                           .ToList();

        //non-aislings only cause partial viewport updates because they do not have shared vision requirements (due to lanterns)
        foreach (var creature in creaturesToUpdate)
            creature.UpdateViewPort([this]);

        var aislingsThatWatchedUsWarp = creaturesToUpdate.ThatAreWithinRange(startPoint)
                                                         .ThatAreWithinRange(destinationPoint)
                                                         .ThatCanObserve(this)
                                                         .OfType<Aisling>();

        foreach (var aisling in aislingsThatWatchedUsWarp)
        {
            HideFrom(aisling);
            ShowTo(aisling);
        }
    }
}
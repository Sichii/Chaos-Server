using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;

namespace Chaos.Models.World.Abstractions;

/// <summary>
///     Represents an object that is visible.
/// </summary>
public abstract class VisibleEntity : MapEntity
{
    public ushort Sprite { get; set; }
    public VisibilityType Visibility { get; set; }

    protected VisibleEntity(ushort sprite, MapInstance mapInstance, IPoint point)
        : base(mapInstance, point) => Sprite = sprite;

    public virtual bool CanObserve(VisibleEntity entity)
    {
        //can always see yourself
        if (entity.Equals(this) || this is Aisling { IsAdmin: true })
            return true;

        switch (entity.Visibility)
        {
            case VisibilityType.Normal:
            case VisibilityType.Hidden:
                return true;
            case VisibilityType.TrueHidden when this is not Creature:
                return false;
            case VisibilityType.TrueHidden when this is Creature creature:
                if (creature is Aisling aisling && (aisling.Group?.Any(member => member.Equals(entity)) == true))
                    return true;

                return creature.Script.CanSee(entity);
            case VisibilityType.GmHidden:
                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

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

    public virtual void HideFrom(Aisling aisling) => aisling.Client.SendRemoveObject(Id);

    public abstract void OnClicked(Aisling source);

    public virtual void SetVisibility(VisibilityType newVisibilityType)
    {
        if (Visibility != newVisibilityType)
        {
            Hide();

            Visibility = newVisibilityType;

            Display();
        }
    }

    public virtual void ShowTo(Aisling aisling) => aisling.Client.SendVisibleEntities(this);

    public override void WarpTo(IPoint destinationPoint)
    {
        Hide();
        SetLocation(destinationPoint);
        Display();
    }
}
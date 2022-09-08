using Chaos.Containers;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;

namespace Chaos.Objects.World.Abstractions;

/// <summary>
///     Represents an object that is visible.
/// </summary>
public abstract class VisibleEntity : MapEntity
{
    public ushort Sprite { get; set; }

    protected VisibleEntity(ushort sprite, MapInstance mapInstance, IPoint point)
        : base(mapInstance, point) => Sprite = sprite;

    public void Display()
    {
        foreach (var aisling in MapInstance.ObjectsThatSee<Aisling>(this))
            ShowTo(aisling);
    }

    public void Hide()
    {
        foreach (var aisling in MapInstance.ObjectsThatSee<Aisling>(this))
            if (!aisling.Equals(this))
                HideFrom(aisling);
    }

    public virtual void HideFrom(Aisling aisling) => aisling.Client.SendRemoveObject(Id);

    public virtual bool IsVisibleTo(Creature creature)
    {
        //can always see yourself
        if (creature.Equals(this))
            return true;

        //TODO: invisibility and other shit
        return true;
    }

    public abstract void OnClicked(Aisling source);

    public virtual void ShowTo(Aisling aisling) => aisling.Client.SendVisibleObjects(this);

    public override string ToString() => $@"({Sprite} - {ILocation.ToString(this)})";

    public override void WarpTo(IPoint destinationPoint)
    {
        Hide();
        SetLocation(destinationPoint);
        Display();
    }
}
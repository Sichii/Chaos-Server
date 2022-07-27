using Chaos.Containers;
using Chaos.Extensions;
using Chaos.Geometry.Interfaces;

namespace Chaos.Objects.World.Abstractions;

/// <summary>
///     Represents an object that is visible.
/// </summary>
public abstract class VisibleEntity : MapEntity
{
    public ushort Sprite { get; set; }
    
    protected VisibleEntity(ushort sprite, MapInstance mapInstance, IPoint point)
        : base(mapInstance, point) => Sprite = sprite;

    public virtual bool IsVisibleTo(Creature creature)
    {
        //can always see yourself
        if (creature.Equals(this))
            return true;

        //TODO: invisibility and other shit
        return true;
    }
    
    public virtual void ShowTo(Aisling aisling) => aisling.Client.SendVisibleObjects(this);
    public virtual void HideFrom(Aisling aisling) => aisling.Client.SendRemoveObject(Id);

    public void Display()
    {
        foreach (var aisling in MapInstance.ObjectsThatSee<Aisling>(this))
            ShowTo(aisling);
    }
    
    public void Hide()
    {
        foreach(var aisling in MapInstance.ObjectsThatSee<Aisling>(this))
            if (!aisling.Equals(this))
                HideFrom(aisling);
    }

    public override void WarpTo(IPoint destinationPoint)
    {
        Hide();
        SetLocation(destinationPoint);
        Display();
    }

    public abstract void OnClicked(Aisling source);

    public override string ToString() => $@"({Sprite} - {ILocation.ToString(this)})";
}
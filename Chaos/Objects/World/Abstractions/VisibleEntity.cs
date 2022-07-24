using Chaos.Containers;
using Chaos.Definitions;
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
    
    public virtual void DisplayTo(Aisling aisling) => aisling.Client.SendVisibleObjects(this);
    public virtual void RemoveFromViewOf(Aisling aisling) => aisling.Client.SendRemoveObject(Id);
    
    public void Display()
    {
        foreach (var aisling in MapInstance.ObjectsThatSee<Aisling>(this))
            DisplayTo(aisling);
    }
    
    public void RemoveFromView()
    {
        foreach(var aisling in MapInstance.ObjectsThatSee<Aisling>(this))
            if (!aisling.Equals(this))
                RemoveFromViewOf(aisling);
    }

    public override void WarpTo(IPoint destinationPoint)
    {
        RemoveFromView();
        SetLocation(destinationPoint);
        Display();
    }

    //public abstract void OnClicked(User source);

    public override string ToString() => $@"({Sprite} - {ILocation.ToString(this)})";
}
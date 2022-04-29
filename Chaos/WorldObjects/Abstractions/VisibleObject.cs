using Chaos.Containers;
using Chaos.Core.Geometry;
using Chaos.Interfaces;

namespace Chaos.WorldObjects.Abstractions;

/// <summary>
///     Represents an object that is visible.
/// </summary>
public abstract class VisibleObject : MapObject, IClickable
{
    public ushort Sprite { get; }

    /// <summary>
    ///     Json & Master constructor for an object that is visible.
    /// </summary>
    protected VisibleObject(MapInstance mapInstance, Point point, ushort sprite)
        : base(mapInstance, point) =>
        Sprite = sprite;

    public abstract void OnClicked(User source);

    public override string ToString() => $@"({Sprite} - {Location})";

    public bool IsVisibleTo(Creature creature)
    {
        //can always see yourself
        if (creature.Equals(this))
            return true;
        
        //TODO: invisibility and other shit
        return true;
    }
}
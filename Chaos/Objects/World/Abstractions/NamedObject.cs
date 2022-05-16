using Chaos.Containers;

namespace Chaos.Objects.World.Abstractions;

public abstract class NamedObject : VisibleObject
{
    public string Name { get; init; }

    protected NamedObject(
        string name,
        MapInstance mapInstance,
        Point point,
        ushort sprite
    )
        : base(mapInstance, point, sprite) => Name = name;
}
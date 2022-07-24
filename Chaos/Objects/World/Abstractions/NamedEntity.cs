using Chaos.Containers;
using Chaos.Geometry.Interfaces;

namespace Chaos.Objects.World.Abstractions;

public abstract class NamedEntity : VisibleEntity
{
    public string Name { get; init; }

    protected NamedEntity(
        string name,
        ushort sprite,
        MapInstance mapInstance,
        IPoint point
    )
        : base(sprite, mapInstance, point) => Name = name;
}
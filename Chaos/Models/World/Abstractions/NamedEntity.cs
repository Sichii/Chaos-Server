using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Models.World.Abstractions;

public abstract class NamedEntity : VisibleEntity, IDeltaUpdatable
{
    public string Name { get; protected set; }

    protected NamedEntity(
        string name,
        ushort sprite,
        MapInstance mapInstance,
        IPoint point)
        : base(sprite, mapInstance, point)
        => Name = name;

    /// <inheritdoc />
    public abstract void Update(TimeSpan delta);
}
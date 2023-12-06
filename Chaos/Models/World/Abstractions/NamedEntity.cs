using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Models.World.Abstractions;

public abstract class NamedEntity(
    string name,
    ushort sprite,
    MapInstance mapInstance,
    IPoint point) : VisibleEntity(sprite, mapInstance, point), IDeltaUpdatable
{
    public string Name { get; protected set; } = name;

    /// <inheritdoc />
    public abstract void Update(TimeSpan delta);
}
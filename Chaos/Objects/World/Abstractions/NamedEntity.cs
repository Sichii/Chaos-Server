using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Objects.World.Abstractions;

public abstract class NamedEntity : VisibleEntity, IDeltaUpdatable
{
    public string Name { get; protected set; }

    protected NamedEntity(
        string name,
        ushort sprite,
        MapInstance mapInstance,
        IPoint point
    )
        : base(sprite, mapInstance, point) =>
        Name = name;

    /// <inheritdoc />
    public override string ToString() => $"{{ Type: \"{GetType().Name}\", Name: \"{Name}\", Loc: \"{ILocation.ToString(this)}\" }}";

    /// <inheritdoc />
    public abstract void Update(TimeSpan delta);
}
using Chaos.Collections;
using Chaos.Geometry.Abstractions;

namespace Chaos.Models.World.Abstractions;

public abstract class InteractableEntity : MapEntity
{
    /// <inheritdoc />
    protected InteractableEntity(MapInstance mapInstance, IPoint point)
        : base(mapInstance, point) { }

    public abstract void OnClicked(Aisling source);
}
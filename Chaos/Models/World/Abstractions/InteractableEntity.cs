#region
using Chaos.Collections;
using Chaos.Geometry.Abstractions;
#endregion

namespace Chaos.Models.World.Abstractions;

public abstract class InteractableEntity : MapEntity
{
    protected ConcurrentDictionary<uint, DateTime> LastClicked { get; init; } = new();

    /// <inheritdoc />
    protected InteractableEntity(MapInstance mapInstance, IPoint point)
        : base(mapInstance, point) { }

    public abstract void OnClicked(Aisling source);

    public virtual bool ShouldRegisterClick(uint fromId)
        => LastClicked.IsEmpty
           || (DateTime.UtcNow.Subtract(LastClicked.Values.Max())
                       .TotalMilliseconds
               > 1500);
}
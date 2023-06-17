using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Abstractions;

public abstract class ReactorTileScriptBase : SubjectiveScriptBase<ReactorTile>, IReactorTileScript
{
    protected MapInstance Map => Subject.MapInstance;
    protected Point Point { get; }

    /// <inheritdoc />
    protected ReactorTileScriptBase(ReactorTile subject)
        : base(subject) => Point = Point.From(Subject);

    /// <inheritdoc />
    public virtual void OnClicked(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnGoldDroppedOn(Creature source, Money money) { }

    /// <inheritdoc />
    public virtual void OnGoldPickedUpFrom(Aisling source, Money money) { }

    /// <inheritdoc />
    public virtual void OnItemDroppedOn(Creature source, GroundItem groundItem) { }

    /// <inheritdoc />
    public virtual void OnItemPickedUpFrom(Aisling source, GroundItem groundItem, int originalCount) { }

    /// <inheritdoc />
    public virtual void OnWalkedOn(Creature source) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}
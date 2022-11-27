using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.ReactorTileScripts.Abstractions;

public abstract class ReactorTileScriptBase : SubjectiveScriptBase<ReactorTile>, IReactorTileScript
{
    /// <inheritdoc />
    protected ReactorTileScriptBase(ReactorTile subject)
        : base(subject) { }

    /// <inheritdoc />
    public virtual void OnClicked(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnGoldDroppedOn(Creature source, Money money) { }

    /// <inheritdoc />
    public virtual void OnGoldPickedUpFrom(Aisling source, Money money) { }

    /// <inheritdoc />
    public virtual void OnItemDroppedOn(Creature source, GroundItem groundItem) { }

    /// <inheritdoc />
    public virtual void OnItemPickedUpFrom(Aisling source, GroundItem groundItem) { }

    /// <inheritdoc />
    public virtual void OnWalkedOn(Creature source) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}
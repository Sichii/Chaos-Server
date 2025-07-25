#region
using Chaos.Collections;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
#endregion

namespace Chaos.Scripting.ItemScripts.Abstractions;

public abstract class ConfigurableItemScriptBase : ConfigurableScriptBase<Item>, IItemScript
{
    /// <inheritdoc />
    protected ConfigurableItemScriptBase(Item subject)
        : base(subject, scriptKey => subject.Template.ScriptVars[scriptKey]) { }

    /// <inheritdoc />
    public virtual bool CanBeDropped(Aisling source, Point targetPoint) => true;

    /// <inheritdoc />
    public virtual bool CanBeDroppedOn(Aisling source, Creature creature) => true;

    /// <inheritdoc />
    public virtual bool CanBePickedUp(Aisling source, Point sourcePoint) => true;

    /// <inheritdoc />
    public virtual bool CanUse(Aisling source) => true;

    /// <inheritdoc />
    public virtual void OnDropped(Creature source, MapInstance mapInstance) { }

    /// <inheritdoc />
    public virtual void OnEquipped(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnNotepadTextUpdated(Aisling source, string? oldText) { }

    /// <inheritdoc />
    public virtual void OnPickup(Aisling aisling, Item originalItem, int originalCount) { }

    /// <inheritdoc />
    public virtual void OnUnEquipped(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnUse(Aisling source) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}
using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.ItemScripts.Abstractions;

public abstract class ItemScriptBase : SubjectiveScriptBase<Item>, IItemScript
{
    /// <inheritdoc />
    protected ItemScriptBase(Item subject)
        : base(subject) { }

    /// <inheritdoc />
    public virtual bool CanUse(Aisling source) => source.IsAlive;

    /// <inheritdoc />
    public virtual void OnDropped(Creature source, MapInstance mapInstance) { }

    /// <inheritdoc />
    public virtual void OnEquipped(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnPickup(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnUnEquipped(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnUse(Aisling source) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}
using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.ItemScripts.Abstractions;

public abstract class ItemScriptBase : ScriptBase, IItemScript
{
    protected Item Subject { get; }

    protected ItemScriptBase(Item subject) => Subject = subject;
    public virtual void OnDropped(Creature source, MapInstance mapInstance) { }
    public virtual void OnEquipped(Aisling aisling) { }
    public virtual void OnPickup(Aisling aisling) { }

    public virtual void OnUnEquipped(Aisling aisling) { }

    public virtual void OnUse(Aisling source) { }
}
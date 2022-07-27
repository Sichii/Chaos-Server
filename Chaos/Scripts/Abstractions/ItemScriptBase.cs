using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Interfaces;

namespace Chaos.Scripts.Abstractions;

public abstract class ItemScriptBase : ScriptBase, IItemScript
{
    protected Item Source { get; }

    protected ItemScriptBase(Item item) => Source = item;

    public abstract void OnUnequip(Aisling aisling);

    public abstract void OnUse(Aisling aisling);
}
using Chaos.PanelObjects;

namespace Chaos.Scripts.Abstractions;

public abstract class ItemScriptBase : ScriptBase<Item>
{
    protected ItemScriptBase(Item item)
        : base(item) { }
}
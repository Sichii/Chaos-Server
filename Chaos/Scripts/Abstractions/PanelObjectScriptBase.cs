using Chaos.DataObjects;
using Chaos.PanelObjects.Abstractions;

namespace Chaos.Scripts.Abstractions;

public abstract class PanelObjectScriptBase : ScriptBase<PanelObjectBase>
{
    protected PanelObjectScriptBase(PanelObjectBase obj)
        : base(obj) { }

    public abstract bool OnUse(ActivationContext activationContext);
}
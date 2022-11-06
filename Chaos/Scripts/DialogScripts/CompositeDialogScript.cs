using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.DialogScripts.Abstractions;

namespace Chaos.Scripts.DialogScripts;

public class CompositeDialogScript : CompositeScriptBase<IDialogScript>, IDialogScript
{
    /// <inheritdoc />
    public virtual void OnClose(Aisling source)
    {
        foreach (var component in Components)
            component.OnClose(source);
    }

    /// <inheritdoc />
    public virtual void OnDisplayed(Aisling source)
    {
        foreach (var component in Components)
            component.OnDisplayed(source);
    }

    /// <inheritdoc />
    public virtual void OnDisplaying(Aisling source)
    {
        foreach (var component in Components)
            component.OnDisplaying(source);
    }

    /// <inheritdoc />
    public virtual void OnNext(Aisling source, byte? optionIndex = null)
    {
        foreach (var component in Components)
            component.OnNext(source, optionIndex);
    }

    /// <inheritdoc />
    public virtual void OnPrevious(Aisling source)
    {
        foreach (var component in Components)
            component.OnPrevious(source);
    }
}
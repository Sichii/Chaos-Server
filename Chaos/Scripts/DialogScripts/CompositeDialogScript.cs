using System.Runtime.InteropServices;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.DialogScripts.Abstractions;

namespace Chaos.Scripts.DialogScripts;

public class CompositeDialogScript : CompositeScriptBase<IDialogScript>, IDialogScript
{
    /// <inheritdoc />
    public virtual void OnDisplayed(Aisling source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnDisplayed(source);
    }

    /// <inheritdoc />
    public virtual void OnDisplaying(Aisling source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnDisplaying(source);
    }

    /// <inheritdoc />
    public virtual void OnNext(Aisling source, byte? optionIndex = null)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnNext(source, optionIndex);
    }

    /// <inheritdoc />
    public virtual void OnPrevious(Aisling source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnPrevious(source);
    }
}
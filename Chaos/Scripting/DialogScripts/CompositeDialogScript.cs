using System.Runtime.InteropServices;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeDialogScript : CompositeScriptBase<IDialogScript>, IDialogScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnDisplayed(Aisling source)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnDisplayed(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnDisplaying(Aisling source)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnDisplaying(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnNext(Aisling source, byte? optionIndex = null)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnNext(source, optionIndex);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnPrevious(Aisling source)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnPrevious(source);
    }
}
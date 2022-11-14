using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.DialogScripts.Abstractions;

public abstract class DialogScriptBase : SubjectiveScriptBase<Dialog>, IDialogScript
{
    /// <inheritdoc />
    protected DialogScriptBase(Dialog subject)
        : base(subject) { }

    /// <inheritdoc />
    public virtual void OnDisplayed(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnDisplaying(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnNext(Aisling source, byte? optionIndex = null) { }

    /// <inheritdoc />
    public virtual void OnPrevious(Aisling source) { }
}
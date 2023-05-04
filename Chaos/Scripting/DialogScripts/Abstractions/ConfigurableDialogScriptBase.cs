using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.DialogScripts.Abstractions;

public abstract class ConfigurableDialogScriptBase : ConfigurableScriptBase<Dialog>, IDialogScript
{
    /// <inheritdoc />
    protected ConfigurableDialogScriptBase(Dialog subject)
        : base(subject, scriptKey => subject.Template.ScriptVars[scriptKey]) { }

    /// <inheritdoc />
    public virtual void OnDisplayed(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnDisplaying(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnNext(Aisling source, byte? optionIndex = null) { }

    /// <inheritdoc />
    public virtual void OnPrevious(Aisling source) { }
}
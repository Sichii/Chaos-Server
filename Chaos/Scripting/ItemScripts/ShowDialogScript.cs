using Chaos.Models.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class ShowDialogScript : ConfigurableItemScriptBase, ShowDialogAbilityComponent.IShowDialogComponentOptions
{
    /// <inheritdoc />
    public ShowDialogScript(Item subject, IDialogFactory dialogFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        DialogSource = Subject;
    }

    /// <inheritdoc />
    public override void OnUse(Aisling source)
        => new ComponentExecutor(source, source).WithOptions(this)
                                                .Execute<ShowDialogAbilityComponent>();

    #region ScriptVars
    public string? DialogKey { get; init; }
    public IDialogFactory DialogFactory { get; init; }
    public IDialogSourceEntity DialogSource { get; init; }
    #endregion
}
using Chaos.Models.Abstractions;
using Chaos.Models.World;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class ShowDialogScript : ConfigurableMerchantScriptBase, ShowDialogAbilityComponent.IShowDialogComponentOptions
{
    /// <inheritdoc />
    public ShowDialogScript(IDialogFactory dialogFactory, Merchant subject)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        DialogSource = Subject;
    }

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
        => new ComponentExecutor(source, source).WithOptions(this)
                                                .Execute<ShowDialogAbilityComponent>();

    #region ScriptVars
    /// <inheritdoc />
    public string? DialogKey { get; init; }

    /// <inheritdoc />
    public IDialogFactory DialogFactory { get; init; }

    /// <inheritdoc />
    public IDialogSourceEntity DialogSource { get; init; }
    #endregion
}
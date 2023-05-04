using Chaos.Models.World;
using Chaos.Scripting.Components;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class ShowDialogScript : ConfigurableMerchantScriptBase, ShowDialogComponent.IShowDialogComponentOptions
{
    #region ScriptVars
    /// <inheritdoc />
    public string DialogKey { get; init; } = null!;
    #endregion

    protected IDialogFactory DialogFactory { get; }
    protected ShowDialogComponent ShowDialogComponent { get; }

    /// <inheritdoc />
    public ShowDialogScript(IDialogFactory dialogFactory, Merchant subject)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        ShowDialogComponent = new ShowDialogComponent(DialogFactory);
    }

    /// <inheritdoc />
    public override void OnClicked(Aisling source) => ShowDialogComponent.ShowDialog(source, Subject, this);
}
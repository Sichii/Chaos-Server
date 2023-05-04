using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.Components;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class ShowDialogScript : ConfigurableItemScriptBase, ShowDialogComponent.IShowDialogComponentOptions
{
    #region ScriptVars
    public string DialogKey { get; init; } = null!;
    #endregion

    protected IDialogFactory DialogFactory { get; }
    protected ShowDialogComponent ShowDialogComponent { get; }

    /// <inheritdoc />
    public ShowDialogScript(Item subject, IDialogFactory dialogFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        ShowDialogComponent = new ShowDialogComponent(DialogFactory);
    }

    /// <inheritdoc />
    public override void OnUse(Aisling source) => ShowDialogComponent.ShowDialog(source, Subject, this);
}
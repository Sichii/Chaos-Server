using Chaos.Factories.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class OpenDialogScript : ConfigurableItemScriptBase
{
    private readonly IDialogFactory DialogFactory;
    protected string DialogKey { get; init; } = null!;

    /// <inheritdoc />
    public OpenDialogScript(Item subject, IDialogFactory dialogFactory)
        : base(subject) =>
        DialogFactory = dialogFactory;

    /// <inheritdoc />
    public override void OnUse(Aisling source)
    {
        var dialog = DialogFactory.Create(DialogKey, Subject);
        dialog.Display(source);
    }
}
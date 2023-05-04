using Chaos.Models.Abstractions;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components;

public class ShowDialogComponent
{
    protected IDialogFactory DialogFactory { get; }

    public ShowDialogComponent(IDialogFactory dialogFactory) => DialogFactory = dialogFactory;

    public virtual void ShowDialog(Aisling aisling, IDialogSourceEntity source, IShowDialogComponentOptions options)
    {
        aisling.DialogHistory.Clear();
        var dialog = DialogFactory.Create(options.DialogKey, source);
        dialog.Display(aisling);
    }

    public interface IShowDialogComponentOptions
    {
        string DialogKey { get; init; }
    }
}
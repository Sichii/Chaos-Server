using Chaos.Objects.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components;

public class ShowDialogComponent
{
    protected IDialogFactory DialogFactory { get; }

    public ShowDialogComponent(IDialogFactory dialogFactory) => DialogFactory = dialogFactory;

    public virtual void ShowDialog(Aisling aisling, object source, IShowDialogComponentOptions options)
    {
        var dialog = DialogFactory.Create(options.DialogKey, source);
        dialog.Display(aisling);
    }

    public interface IShowDialogComponentOptions
    {
        string DialogKey { get; init; }
    }
}
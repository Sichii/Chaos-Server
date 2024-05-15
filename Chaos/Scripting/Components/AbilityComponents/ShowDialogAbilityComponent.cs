using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct ShowDialogAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IShowDialogComponentOptions>();

        if (string.IsNullOrEmpty(options.DialogKey))
            return;

        var targetAisling = context.TargetAisling;

        if (targetAisling == null)
            return;

        targetAisling.DialogHistory.Clear();
        var dialog = options.DialogFactory.Create(options.DialogKey, options.DialogSource);
        dialog.Display(targetAisling);
    }

    public interface IShowDialogComponentOptions
    {
        IDialogFactory DialogFactory { get; init; }
        string? DialogKey { get; init; }
        IDialogSourceEntity DialogSource { get; init; }
    }
}
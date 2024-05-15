using System.Collections.Immutable;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.EffectScripts.HideEffects;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct BreaksHideAbilityComponent : IComponent
{
    private static readonly ImmutableList<string> HideEffects = ImmutableList.Create(
        EffectBase.GetEffectKey(typeof(HideEffect)),
        EffectBase.GetEffectKey(typeof(TrueHideEffect)));

    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IBreaksHideComponentOptions>();

        if (options.ShouldNotBreakHide)
            return;

        if (context.Source.Visibility is not VisibilityType.Normal)
            foreach (var hideEffect in HideEffects)
                context.Source.Effects.Dispel(hideEffect);
    }

    public interface IBreaksHideComponentOptions
    {
        bool ShouldNotBreakHide { get; init; }
    }
}
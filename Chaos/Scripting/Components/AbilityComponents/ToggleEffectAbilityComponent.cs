#region
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;
#endregion

namespace Chaos.Scripting.Components.AbilityComponents;

// ReSharper disable once ClassCanBeSealed.Global
public struct ToggleEffectAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IToggleEffectComponentOptions>();

        if (string.IsNullOrEmpty(options.EffectKey))
            return;

        var targets = vars.GetTargets<Creature>();

        foreach (var target in targets)

            //if they have the effect, dispel it
            if (target.Effects.TryGetEffect(options.EffectKey, out var existingEffect))
                target.Effects.Dispel(existingEffect.Name);

            //otherwise, give them the effect
            else
            {
                var effect = options.EffectFactory.Create(options.EffectKey);

                if (options.EffectDurationOverride.HasValue)
                    effect.SetDuration(options.EffectDurationOverride.Value);

                target.Effects.Apply(context.Source, effect, vars.GetSourceScript());
            }
    }

    public interface IToggleEffectComponentOptions
    {
        TimeSpan? EffectDurationOverride { get; init; }
        IEffectFactory EffectFactory { get; init; }
        string? EffectKey { get; init; }
    }
}
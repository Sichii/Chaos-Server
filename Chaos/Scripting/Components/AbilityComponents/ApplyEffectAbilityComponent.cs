using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct ApplyEffectAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IApplyEffectComponentOptions>();

        if (string.IsNullOrEmpty(options.EffectKey))
            return;

        var targets = vars.GetTargets<Creature>();

        foreach (var target in targets)
        {
            var effect = options.EffectFactory.Create(options.EffectKey);

            if (options.EffectDurationOverride.HasValue)
                effect.SetDuration(options.EffectDurationOverride.Value);

            target.Effects.Apply(context.Source, effect);
        }
    }

    public interface IApplyEffectComponentOptions
    {
        TimeSpan? EffectDurationOverride { get; init; }
        IEffectFactory EffectFactory { get; init; }
        string? EffectKey { get; init; }
    }
}
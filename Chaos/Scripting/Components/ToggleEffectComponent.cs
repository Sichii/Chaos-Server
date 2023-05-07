using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class ToggleEffectComponent : ApplyEffectComponent
{
    /// <inheritdoc />
    public override void Execute(ActivationContext context, ComponentVars vars)
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
                target.Effects.Apply(context.Source, effect);
            }
    }

    public interface IToggleEffectComponentOptions : IApplyEffectComponentOptions { }
}
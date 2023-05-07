using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components;

public class ApplyEffectComponent : IComponent
{
    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IApplyEffectComponentOptions>();

        if (string.IsNullOrEmpty(options.EffectKey))
            return;

        var targets = vars.GetTargets<Creature>();

        foreach (var target in targets)
        {
            var effect = options.EffectFactory.Create(options.EffectKey);
            target.Effects.Apply(context.Source, effect);
        }
    }

    public interface IApplyEffectComponentOptions
    {
        IEffectFactory EffectFactory { get; init; }
        string? EffectKey { get; init; }
    }
}
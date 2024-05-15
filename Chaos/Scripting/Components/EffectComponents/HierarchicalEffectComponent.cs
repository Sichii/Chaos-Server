using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.Components.EffectComponents;

public struct HierarchicalEffectComponent : IConditionalComponent
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var target = context.TargetCreature;

        if (target is null)
            throw new InvalidOperationException("Target is null. This component is intended for use in IEffect.ShouldApply");

        var options = vars.GetOptions<IHierarchicalEffectComponentOptions>();

        var conflictingEffect = target.Effects.FirstOrDefault(e => options.EffectNameHierarchy.Contains(e.Name));

        if (conflictingEffect is null)
            return true;

        var thisRank = options.EffectNameHierarchy.IndexOf(options.Name);
        var conflictingRank = options.EffectNameHierarchy.IndexOf(conflictingEffect.Name);

        if (thisRank <= conflictingRank)
        {
            target.Effects.Dispel(conflictingEffect.Name);

            return true;
        }

        context.SourceAisling?.SendActiveMessage($"Target is already under a stronger effect. [{conflictingEffect.Name}]");

        return false;
    }

    public interface IHierarchicalEffectComponentOptions : IEffect
    {
        List<string> EffectNameHierarchy { get; init; }
    }
}
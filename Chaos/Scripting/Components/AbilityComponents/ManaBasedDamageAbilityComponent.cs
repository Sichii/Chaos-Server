using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct ManaBasedDamageAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IManaBasedDamageComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        var damage = CalculateDamage(context, options);

        if (damage <= 0)
            return;

        foreach (var target in targets)
            options.ApplyDamageScript.ApplyDamage(
                context.Source,
                target,
                options.SourceScript,
                damage,
                options.Element);
    }

    private int CalculateDamage(ActivationContext context, IManaBasedDamageComponentOptions options)
    {
        var baseDamage = options.BaseDamage ?? 0;
        var manaDamage = 0;

        if (options.BaseDamageMultiplier.HasValue)
            baseDamage = Convert.ToInt32(baseDamage * options.BaseDamageMultiplier.Value);

        if (options.PctOfMana.HasValue)
            manaDamage = MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumMp, options.PctOfMana.Value);

        if (options.PctOfManaMultiplier.HasValue)
            manaDamage = Convert.ToInt32(manaDamage * options.PctOfManaMultiplier.Value);

        var finalDamage = baseDamage + manaDamage;

        if (options.FinalMultiplier.HasValue)
            finalDamage = Convert.ToInt32(finalDamage * options.FinalMultiplier.Value);

        return finalDamage;
    }

    public interface IManaBasedDamageComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        int? BaseDamage { get; init; }
        decimal? BaseDamageMultiplier { get; init; }
        Element? Element { get; init; }
        decimal? FinalMultiplier { get; init; }
        decimal? PctOfMana { get; init; }
        decimal? PctOfManaMultiplier { get; init; }
        IScript SourceScript { get; init; }
    }
}
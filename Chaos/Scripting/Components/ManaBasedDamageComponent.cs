using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.Components;

public class ManaBasedDamageComponent
{
    public virtual void ApplyDamage(
        ActivationContext context,
        IReadOnlyCollection<Creature> targetEntities,
        IManaBasedDamageComponentOptions options
    )
    {
        var damage = CalculateDamage(context, options);

        if (damage <= 0)
            return;

        foreach (var target in targetEntities)
            options.ApplyDamageScript.ApplyDamage(
                context.Source,
                target,
                options.SourceScript,
                damage,
                options.Element);
    }

    protected virtual int CalculateDamage(
        ActivationContext context,
        IManaBasedDamageComponentOptions options
    )
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
        IApplyDamageScript ApplyDamageScript { get; }
        int? BaseDamage { get; }
        decimal? BaseDamageMultiplier { get; }
        Element? Element { get; }
        decimal? FinalMultiplier { get; }
        decimal? PctOfMana { get; }
        decimal? PctOfManaMultiplier { get; }
        IScript SourceScript { get; }
    }
}
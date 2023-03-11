using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.Components;

/// <summary>
///     Component that can be used to calculate and deal damage to entities
/// </summary>
public class DamageComponent
{
    public virtual void ApplyDamage(
        ActivationContext context,
        IReadOnlyCollection<Creature> targetEntities,
        IDamageComponentOptions options
    )
    {
        var damage = CalculateDamage(
            context,
            options.BaseDamage,
            options.PctHpDamage,
            options.DamageStat,
            options.DamageStatMultiplier);

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
        int? baseDamage = null,
        decimal? pctHpDamage = null,
        Stat? damageStat = null,
        decimal? damageStatMultiplier = null
    )
    {
        var finalDamage = baseDamage ?? 0;

        finalDamage += MathEx.GetPercentOf<int>((int)context.Target.StatSheet.EffectiveMaximumHp, pctHpDamage ?? 0);

        if (!damageStat.HasValue)
            return finalDamage;

        if (!damageStatMultiplier.HasValue)
        {
            finalDamage += context.Source.StatSheet.GetEffectiveStat(damageStat.Value);

            return finalDamage;
        }

        finalDamage += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(damageStat.Value) * damageStatMultiplier.Value);

        return finalDamage;
    }

    // ReSharper disable once ClassCanBeSealed.Global
    public interface IDamageComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        int? BaseDamage { get; init; }
        Stat? DamageStat { get; init; }
        decimal? DamageStatMultiplier { get; init; }
        Element? Element { get; init; }
        decimal? PctHpDamage { get; init; }
        IScript SourceScript { get; init; }
    }
}
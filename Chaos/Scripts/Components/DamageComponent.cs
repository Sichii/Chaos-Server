using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;

namespace Chaos.Scripts.Components;

/// <summary>
///     Component that can be used to calculate and deal damage to entities
/// </summary>
public class DamageComponent
{
    public virtual void ApplyDamage(ActivationContext context, IReadOnlyCollection<Creature> targetEntities, DamageComponentOptions options)
    {
        var damage = CalculateDamage(
            context,
            options.BaseDamage,
            options.DamageStat,
            options.DamageMultiplier);

        if (damage == 0)
            return;

        foreach (var target in targetEntities)
            options.ApplyDamageScript.ApplyDamage(
                context.Source,
                target,
                options.SourceScript,
                damage);
    }

    protected virtual int CalculateDamage(
        ActivationContext context,
        int? baseDamage = null,
        Stat? damageStat = null,
        decimal? damageStatMultiplier = null
    )
    {
        var finalDamage = baseDamage ?? 0;

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
    public class DamageComponentOptions
    {
        public required IApplyDamageScript ApplyDamageScript { get; init; }
        public int? BaseDamage { get; init; }
        public decimal? DamageMultiplier { get; init; }
        public Stat? DamageStat { get; init; }
        public required IScript SourceScript { get; init; }
    }
}
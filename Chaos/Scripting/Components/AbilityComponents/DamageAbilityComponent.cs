#region
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
#endregion

namespace Chaos.Scripting.Components.AbilityComponents;

public struct DamageAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IDamageComponentOptions>();
        var targets = vars.GetTargets<Creature>();
        var sourceScript = vars.GetSourceScript();

        foreach (var target in targets)
        {
            var damage = CalculateDamage(
                context.Source,
                target,
                options.BaseDamage,
                options.PctHpDamage,
                options.DamageStat,
                options.DamageStatMultiplier);

            if (damage <= 0)
                continue;

            options.ApplyDamageScript.ApplyDamage(
                context.Source,
                target,
                sourceScript,
                damage,
                options.Element);
        }
    }

    private int CalculateDamage(
        Creature source,
        Creature target,
        int? baseDamage = null,
        decimal? pctHpDamage = null,
        Stat? damageStat = null,
        decimal? damageStatMultiplier = null)
    {
        var finalDamage = baseDamage ?? 0;

        finalDamage += MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, pctHpDamage ?? 0);

        if (!damageStat.HasValue)
            return finalDamage;

        if (!damageStatMultiplier.HasValue)
        {
            finalDamage += source.StatSheet.GetEffectiveStat(damageStat.Value);

            return finalDamage;
        }

        finalDamage += Convert.ToInt32(source.StatSheet.GetEffectiveStat(damageStat.Value) * damageStatMultiplier.Value);

        return finalDamage;
    }

    public interface IDamageComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        int? BaseDamage { get; init; }
        Stat? DamageStat { get; init; }
        decimal? DamageStatMultiplier { get; init; }
        Element? Element { get; init; }
        decimal? PctHpDamage { get; init; }
    }
}
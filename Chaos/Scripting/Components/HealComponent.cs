using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.Components;

public class HealComponent
{
    public virtual void ApplyHeal(
        ActivationContext context,
        IReadOnlyCollection<Creature> targetEntities,
        IHealComponentOptions options
    )
    {
        var heal = CalculateHeal(
            context,
            options.BaseHeal,
            options.PctHpHeal,
            options.HealStat,
            options.HealStatMultiplier);

        if (heal == 0)
            return;

        foreach (var target in targetEntities)
            options.ApplyHealScript.ApplyHeal(
                context.Source,
                target,
                options.SourceScript,
                heal);
    }

    protected virtual int CalculateHeal(
        ActivationContext context,
        int? baseHeal = null,
        decimal? pctHpHeal = null,
        Stat? healStat = null,
        decimal? healStatMultiplier = null
    )
    {
        var finalHeal = baseHeal ?? 0;

        finalHeal += MathEx.GetPercentOf<int>((int)context.Target.StatSheet.EffectiveMaximumHp, pctHpHeal ?? 0);

        if (!healStat.HasValue)
            return finalHeal;

        if (!healStatMultiplier.HasValue)
        {
            finalHeal += context.Source.StatSheet.GetEffectiveStat(healStat.Value);

            return finalHeal;
        }

        finalHeal += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(healStat.Value) * healStatMultiplier.Value);

        return finalHeal;
    }

    // ReSharper disable once ClassCanBeSealed.Global
    public interface IHealComponentOptions
    {
        IApplyHealScript ApplyHealScript { get; init; }
        int? BaseHeal { get; init; }
        Stat? HealStat { get; init; }
        decimal? HealStatMultiplier { get; init; }
        decimal? PctHpHeal { get; init; }
        IScript SourceScript { get; init; }
    }
}
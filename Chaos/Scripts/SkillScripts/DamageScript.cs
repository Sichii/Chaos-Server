using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.RuntimeScripts;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class DamageScript : BasicSkillScriptBase
{
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageStatMultiplier { get; init; }

    /// <inheritdoc />
    public DamageScript(Skill subject)
        : base(subject) { }

    protected virtual void ApplyDamage(SkillContext context, IEnumerable<Creature> targetEntities)
    {
        foreach (var target in targetEntities)
            ApplyDamageScripts.Default.ApplyDamage(
                context.Source,
                target,
                this,
                CalculateDamage(context, target));
    }

    protected virtual int CalculateDamage(SkillContext context, Creature target)
    {
        var damage = BaseDamage ?? 0;

        if (DamageStat.HasValue)
        {
            var multiplier = DamageStatMultiplier ?? 1;

            damage += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(DamageStat.Value) * multiplier);
        }

        return damage;
    }

    /// <inheritdoc />
    public override void OnUse(SkillContext context)
    {
        ShowBodyAnimation(context);

        var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
        var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

        ShowAnimation(context, affectedPoints);
        PlaySound(context, affectedPoints);
        ApplyDamage(context, affectedEntities);
    }
}
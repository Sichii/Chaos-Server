using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class StatBasedDamageScript : SkillScriptBase
{
    private readonly int Damage;
    private readonly Stat Stat;
    private readonly decimal StatCoefficient;

    /// <inheritdoc />
    public StatBasedDamageScript(Skill subject)
        : base(subject)
    {
        if (!subject.Template.ScriptVars.TryGetValue(ScriptKey, out var scriptVars))
            throw new InvalidOperationException(
                $"Skill \"{subject.Template.Name}\" does not have script variables for script \"{ScriptKey}\"");

        Damage = scriptVars.Get<int>("damage");
        Stat = scriptVars.Get<Stat>("stat");
        StatCoefficient = scriptVars.Get<decimal>("statCoefficient");
    }

    /// <inheritdoc />
    public override void OnUse(Creature source)
    {
        base.OnUse(source);

        source.AnimateBody(Subject.Template.BodyAnimationOverride ?? BodyAnimation.Assail);

        var point = source.DirectionalOffset(source.Direction);

        var target = source.MapInstance.GetEntitiesAtPoint<Creature>(point)
                           .TopOrDefault();

        if (target == null)
            return;

        var statValue = Stat switch
        {
            Stat.STR => source.StatSheet.EffectiveStr,
            Stat.DEX => source.StatSheet.EffectiveDex,
            Stat.INT => source.StatSheet.EffectiveInt,
            Stat.WIS => source.StatSheet.EffectiveWis,
            Stat.CON => source.StatSheet.EffectiveCon,
            _        => throw new ArgumentOutOfRangeException()
        };

        var damage = Convert.ToInt32(Damage + StatCoefficient * statValue);

        var finalDamage = DamageFormulae.Default.Calculate(source, target, damage);
        target.ApplyDamage(source, finalDamage);
    }
}
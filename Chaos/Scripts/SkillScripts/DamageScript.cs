using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class DamageScript : ConfigurableSkillScriptBase
{
    private readonly int Damage;

    /// <inheritdoc />
    public DamageScript(Skill subject)
        : base(subject) =>
        Damage = ScriptVars.Get<int>("damage");

    /// <inheritdoc />
    public override void OnUse(Creature source)
    {
        source.AnimateBody(Subject.Template.BodyAnimationOverride ?? BodyAnimation.Assail);

        var point = source.DirectionalOffset(source.Direction);

        var target = source.MapInstance.GetEntitiesAtPoint<Creature>(point)
                           .TopOrDefault();

        if (target == null)
            return;

        var damage = DamageFormulae.Default.Calculate(source, target, Damage);
        target.ApplyDamage(source, damage);
    }
}
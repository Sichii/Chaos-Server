using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class DamageScript : ConfigurableSpellScriptBase
{
    private readonly int Damage;

    /// <inheritdoc />
    public DamageScript(Spell subject)
        : base(subject) => Damage = ScriptVars.Get<int>("damage");
    
    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var source = context.Source;
        var target = context.Target;

        if (Subject.Template.Animation != null)
            source.Animate(Subject.Template.Animation, source.Id);
        
        source.AnimateBody(Subject.Template.BodyAnimationOverride ?? BodyAnimation.WizardCast);
        
        var damage = DamageFormulae.Default.Calculate(source, target, Damage);
        
        target.ApplyDamage(source, damage);
    }
}
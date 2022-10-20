using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Formulae;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class DamageScript : ConfigurableSpellScriptBase
{
    protected Animation? Animation { get; init; }
    protected ushort AnimationSpeed { get; init; } = 100;
    protected BodyAnimation? BodyAnimation { get; init; }
    protected int Damage { get; init; }
    protected byte? Sound { get; init; }
    protected ushort? SourceAnimation { get; init; }
    protected ushort? TargetAnimation { get; init; }

    /// <inheritdoc />
    public DamageScript(Spell subject)
        : base(subject)
    {
        if (SourceAnimation.HasValue || TargetAnimation.HasValue)
            Animation = new Animation
            {
                AnimationSpeed = AnimationSpeed,
                SourceAnimation = SourceAnimation ?? 0,
                TargetAnimation = TargetAnimation ?? 0
            };
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        var source = context.Source;
        var target = context.Target;
        var map = source.MapInstance;

        if (BodyAnimation.HasValue)
            source.AnimateBody(BodyAnimation.Value);

        if (Sound.HasValue)
            map.PlaySound(Sound.Value, target);

        if (Animation != null)
            target.Animate(Animation, source.Id);

        var damage = DamageFormulae.Default.Calculate(source, target, Damage);

        target.ApplyDamage(source, damage);
    }
}
using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class DamageScript : ConfigurableSkillScriptBase
{
    protected Animation? Animation { get; init; }
    protected ushort AnimationSpeed { get; init; } = 100;
    protected BodyAnimation? BodyAnimation { get; init; }
    protected int Damage { get; init; }
    protected byte? Sound { get; init; }
    protected ushort? SourceAnimation { get; init; }
    protected ushort? TargetAnimation { get; init; }

    /// <inheritdoc />
    public DamageScript(Skill subject)
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
    public override void OnUse(Creature source)
    {
        if (BodyAnimation.HasValue)
            source.AnimateBody(BodyAnimation.Value);

        var point = source.DirectionalOffset(source.Direction);

        var target = source.MapInstance.GetEntitiesAtPoint<Creature>(point)
                           .TopOrDefault();

        if (target == null)
            return;

        var map = target.MapInstance;

        if (Sound.HasValue)
            map.PlaySound(Sound.Value, target);

        if (Animation != null)
            target.Animate(Animation, source.Id);

        var damage = DamageFormulae.Default.Calculate(source, target, Damage);
        target.ApplyDamage(source, damage);
    }
}
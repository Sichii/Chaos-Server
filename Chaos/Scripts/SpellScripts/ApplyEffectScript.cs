using Chaos.Factories.Abstractions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class ApplyEffectScript : BasicSpellScriptBase
{
    private readonly IEffectFactory EffectFactory;
    protected string EffectKey { get; init; } = null!;

    /// <inheritdoc />
    public ApplyEffectScript(Spell subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        ShowBodyAnimation(context);

        var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
        var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

        ShowAnimation(context, affectedPoints);
        PlaySound(context, affectedPoints);

        foreach (var entity in affectedEntities)
        {
            var effect = EffectFactory.Create(EffectKey);
            entity.Effects.Apply(context.Source, effect);
        }
    }
}
using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.SpellScripts.Abstractions;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public abstract class BasicSpellScriptBase : ConfigurableSpellScriptBase
{
    protected Animation? Animation { get; init; }
    protected BodyAnimation? BodyAnimation { get; init; }
    protected TargetFilter? Filter { get; init; }
    protected int Range { get; init; }
    protected AoeShape Shape { get; init; }
    protected byte? Sound { get; init; }

    /// <inheritdoc />
    protected BasicSpellScriptBase(Spell subject)
        : base(subject) { }

    protected virtual IEnumerable<T> GetAffectedEntities<T>(SpellContext context, IEnumerable<IPoint> affectedPoints) where T: MapEntity
    {
        var entities = context.Map.GetEntitiesAtPoints<T>(affectedPoints);

        if (!Filter.HasValue)
            return entities;

        return entities.Where(entity => entity is not Creature creature || Filter.Value.IsValidTarget(context.Source, creature));
    }

    protected virtual IEnumerable<Point> GetAffectedPoints(SpellContext context) =>
        Shape.ResolvePoints(
            context.TargetPoint,
            Range,
            context.Target.Direction,
            includeSource: (Subject.Template.SpellType == SpellType.Targeted) || (Filter != TargetFilter.HostileOnly));

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        ShowBodyAnimation(context);

        var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
        // ReSharper disable once UnusedVariable
        var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

        ShowAnimation(context, affectedPoints);
        PlaySound(context, affectedPoints);

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }

    protected virtual void PlaySound(SpellContext context, ICollection<IPoint> affectedPoints)
    {
        if (!Sound.HasValue)
            return;

        context.Map.PlaySound(Sound.Value, affectedPoints);
    }

    protected virtual void ShowAnimation(SpellContext context, IEnumerable<IPoint> affectedPoints)
    {
        if (Animation == null)
            return;

        foreach (var point in affectedPoints)
            context.Map.ShowAnimation(Animation.GetPointAnimation(point, context.Source.Id));
    }

    protected virtual void ShowAnimation(SpellContext context, IEnumerable<MapEntity> affectedEntities)
    {
        if (Animation == null)
            return;

        foreach (var entity in affectedEntities)
            entity.Animate(Animation, context.Source.Id);
    }

    protected virtual void ShowBodyAnimation(SpellContext context)
    {
        if (!BodyAnimation.HasValue)
            return;

        context.Source.AnimateBody(BodyAnimation.Value);
    }
}
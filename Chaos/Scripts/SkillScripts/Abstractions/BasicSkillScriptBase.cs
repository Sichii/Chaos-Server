using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.SkillScripts.Abstractions;

public class BasicSkillScriptBase : ConfigurableSkillScriptBase
{
    protected Animation? Animation { get; init; }
    protected BodyAnimation? BodyAnimation { get; init; }
    protected TargetFilter Filter { get; init; }
    protected int Range { get; init; }
    protected AoeShape Shape { get; init; }
    protected byte? Sound { get; init; }

    /// <inheritdoc />
    protected BasicSkillScriptBase(Skill subject)
        : base(subject) { }

    protected virtual IEnumerable<T> GetAffectedEntities<T>(SkillContext context, IEnumerable<IPoint> affectedPoints) where T: MapEntity
    {
        var entities = context.Map.GetEntitiesAtPoints<T>(affectedPoints);

        foreach (var entity in entities)
            if (entity is not Creature c)
                yield return entity;
            else if (Filter.IsValidTarget(context.Source, c))
                yield return entity;
    }

    protected virtual IEnumerable<Point> GetAffectedPoints(SkillContext context) =>
        Shape.ResolvePoints(context.SourcePoint, Range, context.Source.Direction);

    protected virtual void PlaySound(SkillContext context, ICollection<IPoint> affectedPoints)
    {
        if (!Sound.HasValue)
            return;

        context.Map.PlaySound(Sound.Value, affectedPoints);
    }

    protected virtual void ShowAnimation(SkillContext context, IEnumerable<IPoint> affectedPoints)
    {
        if (Animation == null)
            return;

        foreach (var point in affectedPoints)
            context.Map.ShowAnimation(Animation.GetPointAnimation(point, context.Source.Id));
    }

    protected virtual void ShowAnimation(SkillContext context, IEnumerable<MapEntity> affectedEntities)
    {
        if (Animation == null)
            return;

        foreach (var entity in affectedEntities)
            entity.Animate(Animation, context.Source.Id);
    }

    protected virtual void ShowBodyAnimation(SkillContext context)
    {
        if (!BodyAnimation.HasValue)
            return;

        context.Source.AnimateBody(BodyAnimation.Value);
    }
}
using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripting.Components;

public class AbilityComponent
{
    public virtual (IReadOnlyCollection<IPoint> TargetPoints, IReadOnlyCollection<T> TargetEntities) Activate<T>(
        ActivationContext context,
        IAbilityComponentOptions options
    ) where T: MapEntity
    {
        var targetPoints = options.Shape.ResolvePoints(
                                      context.TargetPoint,
                                      options.Range,
                                      context.Target.Direction,
                                      null,
                                      options.IncludeSourcePoint)
                                  .ToListCast<IPoint>();

        var targetEntities = context.Map.GetEntitiesAtPoints<T>(targetPoints)
                                    .WithFilter(context.Source, options.Filter ?? TargetFilter.None)
                                    .ToList();

        if (options.MustHaveTargets && !targetEntities.Any())
            return (targetPoints, targetEntities);

        if (options.BodyAnimation.HasValue)
            context.Source.AnimateBody(options.BodyAnimation.Value);

        if (options.Animation != null)
            if (options.AnimatePoints)
                foreach (var point in targetPoints)
                    context.Map.ShowAnimation(options.Animation.GetPointAnimation(point, context.Source.Id));
            else
                foreach (var target in targetEntities)
                    target.Animate(options.Animation, context.Source.Id);

        if (options.Sound.HasValue)
            context.Map.PlaySound(options.Sound.Value, targetPoints);

        return (targetPoints, targetEntities);
    }

    // ReSharper disable once ClassCanBeSealed.Global
    public interface IAbilityComponentOptions
    {
        bool AnimatePoints { get; init; }
        Animation? Animation { get; init; }
        BodyAnimation? BodyAnimation { get; init; }
        TargetFilter? Filter { get; init; }
        bool IncludeSourcePoint { get; init; }
        bool MustHaveTargets { get; init; }
        int Range { get; init; }
        AoeShape Shape { get; init; }
        byte? Sound { get; init; }
    }
}
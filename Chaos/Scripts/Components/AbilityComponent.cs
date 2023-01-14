using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.Components;

public class AbilityComponent
{
    public virtual (IReadOnlyCollection<IPoint> TargetPoints, IReadOnlyCollection<T> TargetEntities) Activate<T>(
        ActivationContext context,
        AbilityComponentOptions options
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
    public class AbilityComponentOptions
    {
        public bool AnimatePoints { get; init; }
        public Animation? Animation { get; init; }
        public BodyAnimation? BodyAnimation { get; init; }
        public TargetFilter? Filter { get; init; }
        public bool IncludeSourcePoint { get; init; }
        public bool MustHaveTargets { get; init; }
        public required int Range { get; init; }
        public required AoeShape Shape { get; init; }
        public byte? Sound { get; init; }
    }
}
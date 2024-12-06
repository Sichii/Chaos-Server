#region
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
#endregion

namespace Chaos.Scripting.Components.AbilityComponents;

public struct GetTargetsAbilityComponent<TEntity> : IConditionalComponent where TEntity: MapEntity
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IGetTargetsComponentOptions>();
        var map = context.TargetMap;
        var aoeOptions = CreateOptions(context, options);

        var targetPoints = options.Shape
                                  .ResolvePoints(aoeOptions)
                                  .ToList();

        var targetEntities = map.GetEntitiesAtPoints<TEntity>(targetPoints)
                                .WithFilter(context.Source, options.Filter)
                                .ToList();

        if (options.SingleTarget && (targetEntities.Count > 1))
        {
            if (context.TargetCreature is TEntity entity && targetEntities.Contains(entity))
                targetEntities = [entity];
            else
                targetEntities =
                [
                    targetEntities.OrderBy(e => e.Creation)
                                  .First()
                ];
        }

        vars.SetPoints(targetPoints);
        vars.SetTargets(targetEntities);

        return !options.MustHaveTargets || (targetEntities.Count != 0);
    }

    private AoeShapeOptions CreateOptions(ActivationContext context, IGetTargetsComponentOptions options)
    {
        var direction = context.TargetCreature?.Direction ?? context.Target.DirectionalRelationTo(context.Source);

        if (direction == Direction.Invalid)
            direction = context.Source.Direction;

        return new AoeShapeOptions
        {
            Direction = direction,
            ExclusionRange = options.ExclusionRange,
            Range = options.Range,
            Source = context.TargetPoint
        };
    }

    public interface IGetTargetsComponentOptions
    {
        int? ExclusionRange { get; init; }
        TargetFilter Filter { get; init; }
        bool MustHaveTargets { get; init; }
        int Range { get; init; }
        AoeShape Shape { get; init; }
        bool SingleTarget { get; init; }
    }
}
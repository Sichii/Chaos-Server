#region
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
#endregion

namespace Chaos.Scripting.Components.AbilityComponents;

public struct GetCascadingTargetsAbilityComponent<TEntity> : IConditionalComponent where TEntity: MapEntity
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IGetCascadingTargetsComponentOptions>();
        var stage = vars.GetStage();
        List<IPoint> allPoints;

        //if we're on stage 0, get all points for the cascade shape and save them
        if (stage == 0)
        {
            var direction = context.TargetCreature?.Direction ?? context.Target.DirectionalRelationTo(context.Source);

            if (direction == Direction.Invalid)
                direction = context.SnapshotSourceDirection;

            var tempAllPoints = options.Shape
                                       .ResolvePoints(
                                           context.TargetPoint,
                                           options.Range,
                                           direction,
                                           null,
                                           options.IncludeSourcePoint)
                                       .Cast<IPoint>();

            if (options.IgnoreWalls)
                allPoints = tempAllPoints.ToList();
            else
                allPoints = tempAllPoints.FilterByLineOfSight(context.SnapshotSourcePoint, context.TargetMap)
                                         .ToList();

            vars.SetAllPoints(allPoints);
        } else
            allPoints = vars.GetAllPoints();

        //get the slice of points for the current stage
        var stagePoints = options.Shape
                                 .ResolvePointsForRange(
                                     context.SnapshotTargetPoint,
                                     context.SnapshotSourceDirection,
                                     stage,
                                     allPoints)
                                 .ToList();

        var targetEntities = context.TargetMap
                                    .GetEntitiesAtPoints<TEntity>(stagePoints)
                                    .WithFilter(context.Source, options.Filter)
                                    .ToList();

        //set the points and targets for the current stage
        vars.SetPoints(stagePoints);
        vars.SetTargets(targetEntities);

        return !options.MustHaveTargets || (targetEntities.Count != 0);
    }

    public interface IGetCascadingTargetsComponentOptions
    {
        TargetFilter Filter { get; init; }
        bool IgnoreWalls { get; init; }
        bool IncludeSourcePoint { get; init; }
        bool MustHaveTargets { get; init; }
        int Range { get; init; }
        AoeShape Shape { get; init; }
    }
}
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

public struct GetCascadingTargetsAbilityComponent<TEntity> : IConditionalComponent where TEntity: MapEntity
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IGetCascadingTargetsComponentOptions>();
        var stage = vars.GetStage();
        List<Point> allPoints;
        int startingStage;

        if (options.InvertShape)
            startingStage = options.Range;
        else if (options.ExclusionRange.HasValue)
            startingStage = options.ExclusionRange.Value + 1;
        else
            startingStage = 0;

        //if we're on stage 0, get all points for the cascade shape and save them
        if (stage == startingStage)
        {
            var aoeOptions = CreateOptions(context, options);

            var tempAllPoints = options.Shape.ResolvePoints(aoeOptions);

            if (options.IgnoreWalls)
                allPoints = tempAllPoints.ToList();
            else
                allPoints = tempAllPoints.FilterByLineOfSight(context.SnapshotSourcePoint, context.TargetMap, options.InvertShape)
                                         .ToList();

            vars.SetAllPoints(allPoints);
        } else
            allPoints = vars.GetAllPoints();

        var cascadingAoeOptions = CreateOptions(
            context,
            options,
            stage,
            allPoints);

        //get the slice of points for the current stage
        var stagePoints = options.Shape
                                 .ResolvePointsForRange(cascadingAoeOptions)
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

    private AoeShapeOptions CreateOptions(ActivationContext context, IGetCascadingTargetsComponentOptions options)
    {
        var direction = context.SnapshotTargetDirection ?? context.SnapshotTargetPoint.DirectionalRelationTo(context.SnapshotSourcePoint);

        if (direction == Direction.Invalid)
            direction = context.SnapshotSourceDirection;

        return new AoeShapeOptions
        {
            Direction = direction,
            ExclusionRange = options.ExclusionRange,
            Range = options.Range,
            Source = context.SnapshotTargetPoint
        };
    }

    private CascadingAoeShapeOptions CreateOptions(
        ActivationContext context,
        IGetCascadingTargetsComponentOptions options,
        int stage,
        List<Point> allPoints)
    {
        var direction = context.SnapshotTargetDirection ?? context.SnapshotTargetPoint.DirectionalRelationTo(context.SnapshotSourcePoint);

        if (direction == Direction.Invalid)
            direction = context.SnapshotSourceDirection;

        return new CascadingAoeShapeOptions
        {
            Direction = direction,
            ExclusionRange = options.ExclusionRange,
            Range = stage,
            Source = context.SnapshotTargetPoint,
            AllPossiblePoints = allPoints
        };
    }

    public interface IGetCascadingTargetsComponentOptions
    {
        bool ExcludeSourcePoint { get; init; }
        int? ExclusionRange { get; init; }
        TargetFilter Filter { get; init; }
        bool IgnoreWalls { get; init; }
        bool InvertShape { get; init; }
        bool MustHaveTargets { get; init; }
        int Range { get; init; }
        AoeShape Shape { get; init; }
    }
}
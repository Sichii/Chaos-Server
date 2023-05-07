using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class GetTargetsComponent<TEntity> : IConditionalComponent where TEntity: MapEntity
{
    /// <inheritdoc />
    public virtual bool Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IGetTargetsComponentOptions>();
        var direction = context.TargetCreature?.Direction ?? context.Target.DirectionalRelationTo(context.Source);
        var map = context.TargetMap;

        if (direction == Direction.Invalid)
            direction = context.Source.Direction;

        var targetPoints = options.Shape.ResolvePoints(
                                      context.TargetPoint,
                                      options.Range,
                                      direction,
                                      null,
                                      options.ExcludeSourcePoint)
                                  .ToListCast<IPoint>();

        var targetEntities = map.GetEntitiesAtPoints<TEntity>(targetPoints)
                                .WithFilter(context.Source, options.Filter)
                                .ToList();

        vars.SetPoints(targetPoints);
        vars.SetTargets(targetEntities);

        return !options.MustHaveTargets || targetEntities.Any();
    }

    public interface IGetTargetsComponentOptions
    {
        bool ExcludeSourcePoint { get; init; }
        TargetFilter Filter { get; init; }
        bool MustHaveTargets { get; init; }
        int Range { get; init; }
        AoeShape Shape { get; init; }
    }
}
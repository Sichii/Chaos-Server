using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class CascadeDamageScript : DamageScript
{
    protected int MinSoundDelayMs { get; init; }
    protected int PropagationDelayMs { get; init; }
    protected bool StopAtWalls { get; init; }

    /// <inheritdoc />
    public CascadeDamageScript(Spell subject)
        : base(subject) { }

    /// <inheritdoc />
    protected override IEnumerable<Point> GetAffectedPoints(SpellContext context)
    {
        var affectedPoints = base.GetAffectedPoints(context)
                                 .ToList();

        // ReSharper disable once InvertIf
        if (StopAtWalls)
            foreach (var point in affectedPoints.ToList())
                if (context.Map.IsWall(point) || context.Source.RayTraceTo(point).Any(pt => context.Map.IsWall(pt)))
                    affectedPoints.Remove(point);

        return affectedPoints;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        var direction = context.Source.Direction;
        var sourcePoint = Point.From(context.Source);

        ShowBodyAnimation(context);

        var allPossiblePoints = GetAffectedPoints(context)
                                .Cast<IPoint>()
                                .ToList();

        var elapsedMs = MinSoundDelayMs;

        _ = Task.Run(
            async () =>
            {
                for (var i = 1; i <= Range; i++)
                {
                    //get points for this stage
                    var pointsForStage = SelectPointsForStage(
                            allPossiblePoints,
                            sourcePoint,
                            direction,
                            i)
                        .ToList();

                    await using (_ = await context.Map.Sync.WaitAsync())
                    {
                        ShowAnimation(context, pointsForStage);

                        var affectedEntitiesForStage = GetAffectedEntities<Creature>(context, pointsForStage);
                        ApplyDamage(context, affectedEntitiesForStage);

                        if (Sound.HasValue && (elapsedMs >= MinSoundDelayMs))
                        {
                            PlaySound(context, pointsForStage);

                            elapsedMs = 0;
                        }
                    }

                    await Task.Delay(PropagationDelayMs);
                    elapsedMs += PropagationDelayMs;
                }
            });
    }

    private IEnumerable<IPoint> SelectPointsForStage(
        IEnumerable<IPoint> allPossiblePoints,
        Point sourcePoint,
        Direction aoeDirection,
        int range
    )
    {
        switch (Shape)
        {
            case AoeShape.None:
                return Enumerable.Empty<IPoint>();
            case AoeShape.AllAround:
            case AoeShape.Front:
            case AoeShape.FrontalDiamond:
                return allPossiblePoints.Where(pt => pt.DistanceFrom(sourcePoint) == range);

            case AoeShape.FrontalCone:
                var travelsOnXAxis = aoeDirection is Direction.Left or Direction.Right;
                var nextOffset = sourcePoint.DirectionalOffset(aoeDirection, range);

                return allPossiblePoints.Where(pt => travelsOnXAxis ? pt.X == nextOffset.X : pt.Y == nextOffset.Y);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
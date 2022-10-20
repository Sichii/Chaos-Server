using System.Threading.Tasks;
using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class CascadeScript : ConfigurableSpellScriptBase
{
    public enum CascadeShape
    {
        Straight = 1,
        Diamond = 2,
        Cone = 3
    }

    protected Animation? Animation { get; init; }
    protected ushort AnimationSpeed { get; init; } = 100;
    protected BodyAnimation BodyAnimation { get; init; }

    protected int Damage { get; init; }
    protected int MinSoundDelayMs { get; init; }
    protected int PropagationDelayMs { get; init; }
    protected int Range { get; init; }
    protected CascadeShape Shape { get; init; }
    protected byte Sound { get; init; }
    protected ushort? SourceAnimation { get; init; }
    protected bool StopAtWalls { get; init; }
    protected ushort? TargetAnimation { get; init; }

    /// <inheritdoc />
    public CascadeScript(Spell subject)
        : base(subject)
    {
        if (SourceAnimation.HasValue || TargetAnimation.HasValue)
            Animation = new Animation
            {
                AnimationSpeed = AnimationSpeed,
                SourceAnimation = SourceAnimation ?? 0,
                TargetAnimation = TargetAnimation ?? 0
            };
    }

    private void ApplyToPoint(SpellContext context, Point point)
    {
        var map = context.Source.MapInstance;
        var creaturesAtPoint = map.GetEntitiesAtPoint<Monster>(point);

        foreach (var creature in creaturesAtPoint)
            creature.ApplyDamage(context.Source, Damage);

        if (Animation != null)
            map.ShowAnimation(Animation.GetPointAnimation(point));
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        var map = context.Source.MapInstance;
        var direction = context.Source.Direction;
        var sourcePoint = Point.From(context.Source);
        var start = sourcePoint.DirectionalOffset(direction);
        var end = sourcePoint.DirectionalOffset(direction, Range);

        context.Source.AnimateBody(BodyAnimation);

        var allPossiblePoints = Shape switch
        {
            CascadeShape.Straight => start.GetDirectPath(end)
                                          .ToList(),
            CascadeShape.Cone => sourcePoint.ConalSearch(direction, Range)
                                            .ToList(),
            CascadeShape.Diamond => sourcePoint.ConalSearch(direction, Range)
                                               .Where(pt => pt.DistanceFrom(sourcePoint) <= Range)
                                               .ToList(),
            _ => throw new ArgumentOutOfRangeException()
        };

        //prune points based on terrain
        if (StopAtWalls)
            foreach (var point in allPossiblePoints.ToList())
                if (map.IsWall(point) || sourcePoint.RayTraceTo(point).Any(pt => map.IsWall(pt)))
                    allPossiblePoints.Remove(point);

        var elapsedMs = MinSoundDelayMs;

        _ = Task.Run(
            async () =>
            {
                for (var i = 1; i <= Range; i++)
                {
                    var points = SelectPointsForRange(
                            allPossiblePoints,
                            sourcePoint,
                            direction,
                            i)
                        .ToList();

                    foreach (var point in points)
                        ApplyToPoint(context, point);

                    if (elapsedMs >= MinSoundDelayMs)
                    {
                        //anyone who can see any of the points in this layer should hear the sound
                        foreach (var aisling in map.GetEntities<Aisling>()
                                                   .Where(a => points.Any(p => p.WithinRange(a))))
                            aisling.Client.SendSound(Sound, false);

                        elapsedMs = 0;
                    }

                    await Task.Delay(PropagationDelayMs);
                    elapsedMs += PropagationDelayMs;
                }
            });

        base.OnUse(context);
    }

    private IEnumerable<Point> SelectPointsForRange(
        IEnumerable<Point> allPossiblePoints,
        Point sourcePoint,
        Direction aoeDirection,
        int range
    )
    {
        switch (Shape)
        {
            case CascadeShape.Straight:
            case CascadeShape.Diamond:
                return allPossiblePoints.Where(pt => pt.DistanceFrom(sourcePoint) == range);

            case CascadeShape.Cone:
                var travelsOnXAxis = aoeDirection is Direction.Left or Direction.Right;
                var nextOffset = sourcePoint.DirectionalOffset(aoeDirection, range);

                return allPossiblePoints.Where(pt => travelsOnXAxis ? pt.X == nextOffset.X : pt.Y == nextOffset.Y);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
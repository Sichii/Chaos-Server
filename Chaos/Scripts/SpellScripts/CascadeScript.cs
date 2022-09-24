using System.Threading.Tasks;
using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
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

    protected int Damage { get; init; }
    protected byte Sound { get; init; }
    protected BodyAnimation BodyAnimation { get; init; }
    protected int MinSoundDelayMs { get; init; }
    protected int PropagationDelayMs { get; init; }
    protected int Range { get; init; }
    protected CascadeShape Shape { get; init; }
    protected bool StopAtWalls { get; init; }
    protected ushort AnimationSpeed { get; init; } = 100;
    protected ushort? SourceAnimation { get; init; }
    protected ushort? TargetAnimation { get; init; }
    protected Animation? Animation { get; init; }

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

    private void ApplyToPoint(ActivationContext context, Point point)
    {
        var map = context.Source.MapInstance;
        var creaturesAtPoint = map.GetEntitiesAtPoint<Monster>(point);

        foreach (var creature in creaturesAtPoint)
            creature.ApplyDamage(context.Source, Damage);

        if (Animation != null)
            map.ShowAnimation(Animation.GetPointAnimation(point));
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
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
            CascadeShape.Cone => start.ConalSearch(direction, Range)
                                      .ToList(),
            CascadeShape.Diamond => start.ConalSearch(direction, Range)
                                         .Where(pt => pt.DistanceFrom(sourcePoint) <= Range)
                                         .ToList(),
            _ => throw new ArgumentOutOfRangeException()
        };

        //prune points based on terrain
        if (StopAtWalls)
            foreach (var point in allPossiblePoints.ToList())
                if (map.IsWall(point) || sourcePoint.RayTraceTo(point).Any(pt => map.IsWall(pt)))
                    allPossiblePoints.Remove(point);

        var pointSelector = (Func<int, IEnumerable<Point>>)(Shape switch
        {
            CascadeShape.Straight => (r => allPossiblePoints.Where(pt => pt.DistanceFrom(sourcePoint) == r)),
            CascadeShape.Cone => r =>
            {
                var travelsOnXAxis = end.X != start.X;
                var nextOffset = sourcePoint.DirectionalOffset(direction, r);

                return allPossiblePoints.Where(pt => travelsOnXAxis ? pt.X == nextOffset.X : pt.Y == nextOffset.Y);
            },
            CascadeShape.Diamond => r => allPossiblePoints.Where(pt => pt.DistanceFrom(sourcePoint) == r),
            _                    => throw new ArgumentOutOfRangeException()
        });

        var elapsedMs = MinSoundDelayMs;

        _ = Task.Run(
            async () =>
            {
                for (var i = 1; i <= Range; i++)
                {
                    var points = pointSelector(i).ToList();

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
}
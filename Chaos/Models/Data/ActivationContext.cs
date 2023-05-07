using System.Diagnostics;
using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Models.Data;

public interface IActivationContext
{
    Direction Direction { get; }
    Direction SnapshotSourceDirection { get; }
    MapInstance SnapshotSourceMap { get; }
    Point SnapshotSourcePoint { get; }
    MapInstance? SnapshotTargetMap { get; }
    Point SnapshotTargetPoint { get; }
    Creature Source { get; }
    Aisling? SourceAisling { get; }
    MapInstance SourceMap { get; }
    Point SourcePoint { get; }
    IPoint Target { get; }
    Aisling? TargetAisling { get; }
    MapInstance? TargetMap { get; }
    Point TargetPoint { get; }
}

public record ActivationContext : IActivationContext
{
    public Direction SnapshotSourceDirection { get; init; }
    public MapInstance SnapshotSourceMap { get; init; }
    public Point SnapshotSourcePoint { get; init; }
    public Direction? SnapshotTargetDirection { get; init; }
    public MapInstance? SnapshotTargetMap { get; init; }
    public Point SnapshotTargetPoint { get; init; }
    public Creature Source { get; init; }
    public Aisling? SourceAisling { get; init; }
    public Aisling? TargetAisling { get; init; }
    public Creature? TargetCreature { get; init; }
    public Direction Direction => Source.Direction;
    public Direction SourceDirection => Source.Direction;
    public MapInstance SourceMap => Source.MapInstance;
    public Point SourcePoint => Point.From(Source);
    public IPoint Target { get; }

    public Direction TargetDirection
    {
        get
        {
            if (Target is Creature c)
                return c.Direction;

            var relationalDirection = Target.DirectionalRelationTo(Source);

            if (relationalDirection is not Direction.Invalid)
                return relationalDirection;

            return SourceDirection;
        }
    }

    public MapInstance TargetMap => (Target as MapEntity)?.MapInstance
                                    ?? SnapshotTargetMap
                                    ?? throw new UnreachableException(
                                        "Target map should always be populated. "
                                        + "Either the target should be an entity with a map, "
                                        + "or it should be a point that was passed in with a map in it's constructor");
    public Point TargetPoint => Point.From(Target);

    public ActivationContext(Creature source, MapEntity target)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Target = target ?? throw new ArgumentNullException(nameof(target));
        SourceAisling = Source as Aisling;
        TargetAisling = Target as Aisling;
        TargetCreature = Target as Creature;
        SnapshotSourceMap = Source.MapInstance;
        SnapshotTargetMap = (Target as MapEntity)?.MapInstance;
        SnapshotSourcePoint = Point.From(Source);
        SnapshotTargetPoint = Point.From(Target);
        SnapshotSourceDirection = Source.Direction;
        SnapshotTargetDirection = (Target as Creature)?.Direction ?? target.DirectionalRelationTo(source);

        if (SnapshotTargetDirection is Direction.Invalid)
            SnapshotTargetDirection = SnapshotSourceDirection;
    }

    public ActivationContext(Creature source, IPoint target, MapInstance map)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Target = target ?? throw new ArgumentNullException(nameof(target));
        SourceAisling = Source as Aisling;
        TargetAisling = Target as Aisling;
        TargetCreature = Target as Creature;
        SnapshotSourceMap = Source.MapInstance;
        SnapshotTargetMap = map ?? throw new ArgumentNullException(nameof(map));
        SnapshotSourcePoint = Point.From(Source);
        SnapshotTargetPoint = Point.From(Target);
        SnapshotSourceDirection = Source.Direction;
    }
}
#region
using System.Diagnostics;
using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
#endregion

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
    /// <summary>
    ///     The direction the source was facing at time of activation
    /// </summary>
    public Direction SnapshotSourceDirection { get; init; }

    /// <summary>
    ///     The map the source was on at time of activation
    /// </summary>
    public MapInstance SnapshotSourceMap { get; init; }

    /// <summary>
    ///     The point the source was at at time of activation
    /// </summary>
    public Point SnapshotSourcePoint { get; init; }

    /// <summary>
    ///     The direction the target was facing at time of activation
    /// </summary>
    public Direction? SnapshotTargetDirection { get; init; }

    /// <summary>
    ///     The map the target was on at time of activation
    /// </summary>
    public MapInstance? SnapshotTargetMap { get; init; }

    /// <summary>
    ///     The point the target was at at time of activation
    /// </summary>
    public Point SnapshotTargetPoint { get; init; }

    /// <summary>
    ///     The source of the activation
    /// </summary>
    public Creature Source { get; init; }

    /// <summary>
    ///     The aisling source of the activation (if applicable)
    /// </summary>
    public Aisling? SourceAisling { get; init; }

    /// <summary>
    ///     The aisling target of the activation (if applicable)
    /// </summary>
    public Aisling? TargetAisling { get; init; }

    /// <summary>
    ///     The creature target of the activation (if applicable)
    /// </summary>
    public Creature? TargetCreature { get; init; }

    /// <summary>
    ///     The target of the activation
    /// </summary>
    public IPoint Target { get; }

    /// <summary>
    ///     The direction of the target. If the target is a creature, this will be the direction the creature is currently
    ///     facing. If the target is a point, this will be the relative direction from the source to the target.
    /// </summary>
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

    /// <summary>
    ///     The direction the source is currently facing
    /// </summary>
    public Direction Direction => Source.Direction;

    /// <summary>
    ///     The direction the source is currently facing
    /// </summary>
    public Direction SourceDirection => Source.Direction;

    /// <summary>
    ///     The map the source is currently on
    /// </summary>
    public MapInstance SourceMap => Source.MapInstance;

    /// <summary>
    ///     The point the source is currently at
    /// </summary>
    public Point SourcePoint => Point.From(Source);

    /// <summary>
    ///     The target map of the activation. If the target is a creature, this will be the map the creature is currently on.
    ///     Otherwise, it will be the map the target was on at time of activation.
    /// </summary>
    public MapInstance TargetMap
        => (Target as MapEntity)?.MapInstance
           ?? SnapshotTargetMap
           ?? throw new UnreachableException(
               "Target map should always be populated. "
               + "Either the target should be an entity with a map, "
               + "or it should be a point that was passed in with a map in it's constructor");

    /// <summary>
    ///     The target point of the activation. If the target is a creature, this will be the point the creature is currently
    ///     at.
    /// </summary>
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
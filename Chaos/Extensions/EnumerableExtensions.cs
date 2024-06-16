using System.Runtime.CompilerServices;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Extensions;

public static class EnumerableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? ClosestOrDefault<T>(this IEnumerable<T> objs, IPoint point) where T: MapEntity
        => objs.MinBy(o => o.DistanceFrom(point));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Item> FixStacks(this IEnumerable<Item> items, ICloningService<Item> itemCloner)
        => items.GroupBy(i => i.DisplayName, (_, s) => s.ToSingleStack())
                .SelectMany(i => i.FixStacks(itemCloner));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatAreIlluminatedBy<T>(this IEnumerable<T> objs, Aisling aisling) where T: VisibleEntity
        => objs.Where(aisling.Illuminates);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatAreInLanternVision<T>(this IEnumerable<T> objs) where T: VisibleEntity
        => objs.Where(obj => obj.MapInstance.IsInSharedLanternVision(obj));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatAreObservedBy<T>(this IEnumerable<T> objs, Creature creature, bool fullCheck = false)
        where T: VisibleEntity
        => objs.Where(obj => creature.CanObserve(obj, fullCheck));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatAreOnPoint<T>(this IEnumerable<T> objs, IPoint point) where T: MapEntity
        => objs.Where(obj => PointEqualityComparer.Instance.Equals(obj, point));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatAreVisibleTo<T>(this IEnumerable<T> objs, Creature creature) where T: VisibleEntity
        => objs.Where(creature.CanSee);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatAreWithinRange<T>(this IEnumerable<T> objs, ILocation location, int range = 15) where T: MapEntity
        => objs.Where(obj => obj.WithinRange(location, range));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatAreWithinRange<T>(this IEnumerable<T> objs, IPoint point, int range = 15) where T: MapEntity
        => objs.Where(obj => obj.WithinRange(point, range));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatAreWithinRange<T>(this IEnumerable<T> objs, int range = 15, params IPoint[] points) where T: MapEntity
        => objs.Where(obj => points.Any(point => obj.WithinRange(point, range)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatAreWithinRange<T>(this IEnumerable<T> objs, int range = 15, params ILocation[] locations)
        where T: MapEntity
        => objs.Where(obj => locations.Any(point => obj.WithinRange(point, range)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatCanObserve<T>(this IEnumerable<T> objs, VisibleEntity visibleEntity, bool fullCheck = false)
        where T: VisibleEntity
        => objs.Where(obj => obj is Creature creature && creature.CanObserve(visibleEntity, fullCheck));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatCanSee<T>(this IEnumerable<T> objs, VisibleEntity visibleEntity) where T: Creature
        => objs.Where(obj => obj.CanSee(visibleEntity));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatCollideWith<T>(this IEnumerable<T> objs, Creature creature) where T: Creature
        => objs.Where(obj => !obj.Equals(creature) && obj.WillCollideWith(creature));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ThatThisCollidesWith<T>(this IEnumerable<T> objs, Creature creature) where T: Creature
        => objs.Where(obj => !obj.Equals(creature) && creature.WillCollideWith(obj));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? TopOrDefault<T>(this IEnumerable<T> objs) where T: WorldEntity => objs.MaxBy(o => o.Creation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Item ToSingleStack(this IEnumerable<Item> items)
        => items.Aggregate(
            (item1, item2) =>
            {
                item1.Count += item2.Count;

                return item1;
            });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WithFilter<T>(this IEnumerable<T> objs, Creature source, TargetFilter filter) where T: MapEntity
        => objs.Where(obj => obj is not Creature creature || filter.IsValidTarget(source, creature));
}
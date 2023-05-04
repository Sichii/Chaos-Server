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
    public static T? ClosestOrDefault<T>(this IEnumerable<T> objs, IPoint point) where T: MapEntity =>
        objs.MinBy(o => o.DistanceFrom(point));

    public static IEnumerable<Item> FixStacks(this IEnumerable<Item> items, ICloningService<Item> itemCloner) => items
        .GroupBy(i => i.DisplayName, (_, s) => s.ToSingleStack())
        .SelectMany(i => i.FixStacks(itemCloner));

    public static (ICollection<Aisling> Aislings, ICollection<Door> Doors, ICollection<VisibleEntity> OtherVisibles) PartitionBySendType(
        this IEnumerable<VisibleEntity> visibleEntities
    )
    {
        var aislings = new List<Aisling>();
        var doors = new List<Door>();
        var others = new List<VisibleEntity>();

        foreach (var obj in visibleEntities)
            switch (obj)
            {
                case Aisling userObj:
                    aislings.Add(userObj);

                    break;
                case Creature:
                case GroundEntity:
                    others.Add(obj);

                    break;
                case Door doorObj:
                    doors.Add(doorObj);

                    break;
            }

        return (aislings, doors, others);
    }

    public static IEnumerable<T> ThatAreObservedBy<T>(this IEnumerable<T> objs, VisibleEntity visibleEntity) where T: VisibleEntity =>
        objs.Where(visibleEntity.CanObserve);

    public static IEnumerable<T> ThatAreOnPoint<T>(this IEnumerable<T> objs, IPoint point) where T: MapEntity
        => objs.Where(o => PointEqualityComparer.Instance.Equals(o, point));

    public static IEnumerable<T> ThatAreWithinRange<T>(this IEnumerable<T> objs, ILocation point, int range = 12) where T: MapEntity
        => objs.Where(o => o.WithinRange(point, range));

    public static IEnumerable<T> ThatAreWithinRange<T>(this IEnumerable<T> objs, IPoint point, int range = 12) where T: MapEntity
        => objs.Where(o => o.WithinRange(point, range));

    public static IEnumerable<T> ThatCanObserve<T>(this IEnumerable<T> objs, VisibleEntity visibleEntity) where T: Creature =>
        objs.Where(obj => obj.CanObserve(visibleEntity));

    public static IEnumerable<T> ThatCollideWith<T>(this IEnumerable<T> objs, Creature creature) where T: Creature =>
        objs.Where(c => !c.Equals(creature) && creature.WillCollideWith(c));

    public static T? TopOrDefault<T>(this IEnumerable<T> objs) where T: WorldEntity => objs.MaxBy(o => o.Creation);

    public static Item ToSingleStack(this IEnumerable<Item> items) =>
        items.Aggregate(
            (item1, item2) =>
            {
                item1.Count += item2.Count;

                return item1;
            });

    public static IEnumerable<T> WithFilter<T>(this IEnumerable<T> objs, Creature source, TargetFilter filter) where T: MapEntity =>
        objs.Where(o => o is not Creature creature || filter.IsValidTarget(source, creature));
}
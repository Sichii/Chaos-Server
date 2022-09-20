using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Services.Utility.Abstractions;

namespace Chaos.Extensions;

public static class EnumerableExtensions
{
    public static T? ClosestOrDefault<T>(this IEnumerable<T> objs, IPoint point) where T: MapEntity =>
        objs.MinBy(o => o.DistanceFrom(point));

    public static IEnumerable<Item> FixStacks(this IEnumerable<Item> items, ICloningService<Item> itemCloner) =>
        items.SelectMany(item => item.FixStacks(itemCloner));

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

    public static IEnumerable<T> ThatAreOnPoint<T>(this IEnumerable<T> objs, IPoint point) where T: MapEntity
        => objs.Where(o => PointEqualityComparer.Instance.Equals(o, point));

    public static IEnumerable<T> ThatAreVisibleTo<T>(this IEnumerable<T> objs, Creature creature) where T: VisibleEntity =>
        objs.Where(obj => obj.IsVisibleTo(creature));

    public static IEnumerable<T> ThatAreWithinRange<T>(this IEnumerable<T> objs, IPoint point, int range = 13) where T: MapEntity
        => objs.Where(o => o.WithinRange(point, range));

    public static IEnumerable<T> ThatCanSee<T>(this IEnumerable<T> objs, VisibleEntity visibleEntity) where T: Creature =>
        objs.Where(visibleEntity.IsVisibleTo);

    public static IEnumerable<T> ThatCollideWith<T>(this IEnumerable<T> objs, Creature creature) where T: Creature =>
        objs.Where(c => !c.Equals(creature) && c.WillCollideWith(creature));

    public static T? TopOrDefault<T>(this IEnumerable<T> objs) where T: WorldEntity => objs.MaxBy(o => o.Creation);

    public static Item ToSingleStack(this IEnumerable<Item> items) =>
        items.Aggregate(
            (item1, item2) =>
            {
                item1.Count += item2.Count;

                return item1;
            });
}
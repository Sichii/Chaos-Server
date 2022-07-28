using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Services.Utility.Interfaces;

namespace Chaos.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<Item> FixStacks(this IEnumerable<Item> items, ICloningService<Item> itemCloner) =>
        items.SelectMany(item => item.FixStacks(itemCloner));

    public static (ICollection<Aisling> Aislings, ICollection<Door> Doors, ICollection<VisibleEntity> OtherVisibles) SortBySendType(
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
                case Creature creatureObj:
                    others.Add(creatureObj);

                    break;
                case GroundItem groundItemObj:
                    others.Add(groundItemObj);

                    break;
                case Money money:
                    others.Add(money);

                    break;
                case Door doorObj:
                    doors.Add(doorObj);

                    break;
            }

        return (aislings, doors, others);
    }

    public static Item ToSingleStack(this IEnumerable<Item> items) =>
        items.Aggregate(
            (item1, item2) =>
            {
                item1.Count += item2.Count;

                return item1;
            });
}
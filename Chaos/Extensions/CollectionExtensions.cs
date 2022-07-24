using Chaos.Objects.World.Abstractions;

namespace Chaos.Extensions;

public static class CollectionExtensions
{
    public static IEnumerable<T> ThatAreVisibleTo<T>(this IEnumerable<T> objs, Creature creature) where T: VisibleEntity =>
        objs.Where(obj => obj.IsVisibleTo(creature));

    public static IEnumerable<T> ThatCanSee<T>(this IEnumerable<T> objs, VisibleEntity visibleEntity) where T: Creature =>
        objs.Where(visibleEntity.IsVisibleTo);
}
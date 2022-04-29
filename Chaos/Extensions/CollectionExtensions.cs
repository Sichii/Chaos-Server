using System.Collections.Generic;
using System.Linq;
using Chaos.WorldObjects.Abstractions;

namespace Chaos.Extensions;

public static class CollectionExtensions
{
    public static IEnumerable<T> ThatAreVisibleTo<T>(this IEnumerable<T> objs, Creature creature) where T: VisibleObject =>
        objs.Where(obj => obj.IsVisibleTo(creature));

    public static IEnumerable<T> ThatCanSee<T>(this IEnumerable<T> objs, VisibleObject visibleObject) where T: Creature =>
        objs.Where(visibleObject.IsVisibleTo);
}
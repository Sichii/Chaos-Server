namespace Chaos.Core.Collections;

public class TypeSortedCollection
{
    private readonly ConcurrentDictionary<Type, IEnumerable> TypeCollections;

    public TypeSortedCollection(IEnumerable<object> objects, params Type[] types)
    {
        var objectColl = objects.ToList();
        
        TypeCollections = new ConcurrentDictionary<Type, IEnumerable>();
        var genericList = typeof(List<>);
        
        //populate type collections
        foreach (var type in types)
        {
            var typeCol = (IList)TypeCollections.GetOrAdd(type, (IEnumerable)Activator.CreateInstance(genericList.MakeGenericType(type))!);
            
            foreach(var obj in objectColl)
                if (type.IsInstanceOfType(obj))
                    typeCol.Add(obj);
        }
    }

    public ICollection<T> Get<T>() => TypeCollections.TryGetValue(typeof(T), out var typeCol) ? (ICollection<T>)typeCol : Array.Empty<T>();
}
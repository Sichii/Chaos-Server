using System.Threading;

namespace Chaos.Objects;

public class ActiveObject
{
    private object? Object;

    public bool TrySet(object obj) => Interlocked.CompareExchange(ref Object, obj, null) is null;

    public T? TryGet<T>() where T: class => Object as T;

    public bool TryRemove(object obj) => ReferenceEquals(Interlocked.CompareExchange(ref Object, null, obj), obj);
}
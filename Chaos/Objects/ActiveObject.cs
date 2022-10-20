using System.Threading;

namespace Chaos.Objects;

public sealed class ActiveObject
{
    private object? Object;

    public T? TryGet<T>() where T: class => Object as T;

    public bool TryRemove(object obj) => ReferenceEquals(Interlocked.CompareExchange(ref Object, null, obj), obj);

    public bool TrySet(object obj) => Interlocked.CompareExchange(ref Object, obj, null) is null;

    public bool TryReplace(object @new, object old) => ReferenceEquals(Interlocked.CompareExchange(ref Object, @new, old), old);
}
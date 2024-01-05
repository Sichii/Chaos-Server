using Chaos.Common.Abstractions;

namespace Chaos.Common.Synchronization;

/// <summary>
///     A threadsafe object wrapper
/// </summary>
/// <typeparam name="T">
///     The type of the object to wrap
/// </typeparam>
public class InterlockedObject<T> : IInterlockedObject<T> where T: class
{
    /// <summary>
    ///     The object to be accessed in a threadsafe manner
    /// </summary>
    protected T? Object;

    /// <inheritdoc />
    public virtual T? Get() => Object;

    /// <inheritdoc />
    public void Set(T? obj) => Object = obj;

    /// <inheritdoc />
    public virtual bool SetIfNull(T obj) => Interlocked.CompareExchange(ref Object, obj, null) is null;

    /// <inheritdoc />
    public virtual bool TryRemove(T old) => ReferenceEquals(Interlocked.CompareExchange(ref Object, null, old), old);

    /// <inheritdoc />
    public virtual bool TryReplace(T @new, T old) => ReferenceEquals(Interlocked.CompareExchange(ref Object, @new, old), old);
}
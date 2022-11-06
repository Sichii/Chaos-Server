namespace Chaos.Common.Abstractions;

public interface IInterlockedObject<T> where T: class
{
    T? Get();
    void Set(T obj);
    bool SetIfNull(T obj);
    bool TryRemove(T old);
    bool TryReplace(T @new, T old);
}
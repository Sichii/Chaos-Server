namespace Chaos.Storage.Abstractions;

public interface ISaveManager<T>
{
    Task<T> LoadAsync(string key);
    Task SaveAsync(T user);
}
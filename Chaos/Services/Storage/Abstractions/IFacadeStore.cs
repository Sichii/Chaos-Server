namespace Chaos.Services.Storage.Abstractions;

public interface IFacadeStore<T>
{
    T Load(string key);

    Task<T> LoadAsync(string key);
}
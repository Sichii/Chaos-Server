namespace Chaos.Storage.Abstractions;

/// <summary>
///     Defines the methods that are required to save and load objects asynchronously
/// </summary>
/// <typeparam name="T">The type of the objects to be saved and loaded</typeparam>
public interface IAsyncStore<T>
{
    /// <summary>
    ///     Asynchronously checks if an object with the specified key exists, whether it is loaded or not
    /// </summary>
    /// <param name="key">The key to check</param>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    ///     Loads an object with the specified key
    /// </summary>
    /// <param name="key">A key that is unique to the object to be loaded</param>
    /// <returns>A task that returns the object</returns>
    Task<T> LoadAsync(string key);

    /// <summary>
    ///     Saves an object
    /// </summary>
    /// <param name="obj">The object to be saved</param>
    Task SaveAsync(T obj);
}
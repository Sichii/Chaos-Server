namespace Chaos.Storage.Abstractions;

/// <summary>
///     Provides methods for loading and saving objects from storage
/// </summary>
public interface IStorageManager
{
    /// <summary>
    ///     Loads an object from storage
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the object to load
    /// </typeparam>
    IStorage<T> Load<T>() where T: class, new();

    /// <summary>
    ///     Loads an object from storage asynchronously
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the object to load
    /// </typeparam>
    Task<IStorage<T>> LoadAsync<T>() where T: class, new();

    /// <summary>
    ///     Saves an object to storage
    /// </summary>
    /// <param name="obj">
    ///     The object to save
    /// </param>
    /// <typeparam name="T">
    ///     The type of the object to save
    /// </typeparam>
    void Save<T>(IStorage<T> obj) where T: class, new();

    /// <summary>
    ///     Saves an object to storage asynchronously
    /// </summary>
    /// <param name="obj">
    ///     The object to save
    /// </param>
    /// <typeparam name="T">
    ///     The type of the object to save
    /// </typeparam>
    Task SaveAsync<T>(IStorage<T> obj) where T: class, new();
}
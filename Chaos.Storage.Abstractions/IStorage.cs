namespace Chaos.Storage.Abstractions;

/// <summary>
///     A wrapper around an object loaded from storage that can be saved
/// </summary>
/// <typeparam name="T">
///     The wrapped type
/// </typeparam>
public interface IStorage<out T> : IReadOnlyStorage<T> where T: class, new()
{
    /// <summary>
    ///     Gets a named instance of the wrapped object
    /// </summary>
    /// <param name="name">
    ///     The named instance of the object
    /// </param>
    new IStorage<T> GetInstance(string name);

    /// <summary>
    ///     Saves the wrapped object to storage
    /// </summary>
    void Save();

    /// <summary>
    ///     Saves the wrapped object to storage asynchronously
    /// </summary>
    Task SaveAsync();
}
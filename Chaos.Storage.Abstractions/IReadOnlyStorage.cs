namespace Chaos.Storage.Abstractions;

/// <summary>
///     A wrapper around an object loaded from storage that is read only
/// </summary>
/// <typeparam name="T">
///     The wrapped type
/// </typeparam>
public interface IReadOnlyStorage<out T> where T: class, new()
{
    /// <summary>
    ///     The default instance of the wrapped object
    /// </summary>
    T Value { get; }

    /// <summary>
    ///     Gets a named instance of the wrapped object
    /// </summary>
    /// <param name="name">
    ///     The named instance of the object
    /// </param>
    IReadOnlyStorage<T> GetInstance(string name);
}
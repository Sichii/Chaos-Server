namespace Chaos.TypeMapper.Abstractions;

/// <summary>
///     Defines a pattern for cloning objects
/// </summary>
/// <typeparam name="T">
///     The type of object that will be clonsd
/// </typeparam>
public interface ICloningService<T>
{
    /// <summary>
    ///     Clones the given objects
    /// </summary>
    /// <param name="obj">
    ///     The object to cone
    /// </param>
    T Clone(T obj);
}
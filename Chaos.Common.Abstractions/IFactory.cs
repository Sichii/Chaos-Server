namespace Chaos.Common.Abstractions;

/// <summary>
///     Represents a factory that can create an instance of <typeparamref name="T" />.
/// </summary>
/// <typeparam name="T">
///     The type of object to create
/// </typeparam>
public interface IFactory<out T>
{
    /// <summary>
    ///     Creates an instance of <typeparamref name="T" />.
    /// </summary>
    /// <param name="args">
    ///     The arguments required to create the object
    /// </param>
    T Create(params object[] args);
}
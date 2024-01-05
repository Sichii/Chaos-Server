using System.Diagnostics.CodeAnalysis;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines the pattern for a service that clients register to so that they are more easily discovered
/// </summary>
/// <typeparam name="T">
///     A type of client implementing <see cref="Chaos.Networking.Abstractions.ISocketClient" />
/// </typeparam>
public interface IClientRegistry<T> : IEnumerable<T> where T: ISocketClient
{
    /// <summary>
    ///     Gets a client by it's id from the registry
    /// </summary>
    /// <param name="id">
    ///     The id of the client
    /// </param>
    T? GetClient(uint id);

    /// <summary>
    ///     Adds a client to the registry
    /// </summary>
    /// <param name="client">
    ///     The client to add
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the client was successfully added, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryAdd(T client);

    /// <summary>
    ///     Removes a client from the registry
    /// </summary>
    /// <param name="id">
    ///     The id of the client
    /// </param>
    /// <param name="client">
    ///     If successful, the client that was removed
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a client with the id was found and removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>

    // ReSharper disable once OutParameterValueIsAlwaysDiscarded.Global
    bool TryRemove(uint id, [MaybeNullWhen(false)] out T client);
}
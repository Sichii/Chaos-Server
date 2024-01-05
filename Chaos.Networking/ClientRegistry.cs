using Chaos.Networking.Abstractions;

namespace Chaos.Networking;

/// <summary>
///     A registry that facilitates discovery of clients
/// </summary>
/// <typeparam name="T">
/// </typeparam>
public class ClientRegistry<T> : IClientRegistry<T> where T: ISocketClient
{
    /// <summary>
    ///     The clients that are currently registered
    /// </summary>
    protected ConcurrentDictionary<uint, T> Clients { get; }

    /// <summary>
    ///     Creates a new instance of <see cref="ClientRegistry{T}" />
    /// </summary>
    public ClientRegistry() => Clients = new ConcurrentDictionary<uint, T>();

    /// <inheritdoc />
    public virtual T? GetClient(uint id) => Clients.TryGetValue(id, out var client) ? client : default;

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public virtual IEnumerator<T> GetEnumerator() => Clients.Values.GetEnumerator();

    /// <inheritdoc />
    public virtual bool TryAdd(T client) => Clients.TryAdd(client.Id, client);

    /// <inheritdoc />
    public virtual bool TryRemove(uint id, [MaybeNullWhen(false)] out T client) => Clients.Remove(id, out client);
}
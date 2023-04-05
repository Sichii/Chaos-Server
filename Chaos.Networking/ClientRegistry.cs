using Chaos.Networking.Abstractions;

namespace Chaos.Networking;

/// <summary>
///     A registry that facilitates discovery of clients
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ClientRegistry<T> : IClientRegistry<T> where T: ISocketClient
{
    private readonly ConcurrentDictionary<uint, T> Clients;

    /// <summary>
    ///     Creates a new instance of <see cref="ClientRegistry{T}" />
    /// </summary>
    public ClientRegistry() => Clients = new ConcurrentDictionary<uint, T>();

    /// <inheritdoc />
    public T? GetClient(uint id) => Clients.TryGetValue(id, out var client) ? client : default;

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => Clients.Values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public bool TryAdd(T client) => Clients.TryAdd(client.Id, client);

    /// <inheritdoc />
    public bool TryRemove(uint id, [MaybeNullWhen(false)] out T client) => Clients.Remove(id, out client);
}
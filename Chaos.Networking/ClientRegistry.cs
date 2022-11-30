using Chaos.Networking.Abstractions;

namespace Chaos.Networking;

public sealed class ClientRegistry<T> : IClientRegistry<T> where T: ISocketClient
{
    private readonly ConcurrentDictionary<uint, T> Clients;

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
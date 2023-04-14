using Chaos.Networking.Abstractions;

namespace Chaos.Networking.Entities;

/// <inheritdoc />
public sealed class RedirectManager : IRedirectManager
{
    private readonly ConcurrentDictionary<uint, IRedirect> Redirects = new();

    /// <inheritdoc />
    public void Add(IRedirect redirect) => Redirects.TryAdd(redirect.Id, redirect);

    /// <inheritdoc />
    public bool TryGetRemove(uint id, out IRedirect redirect) => Redirects.TryRemove(id, out redirect!);
}
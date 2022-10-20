using Chaos.Networking.Abstractions;

namespace Chaos.Networking.Entities;

public sealed class RedirectManager : IRedirectManager
{
    private readonly ConcurrentDictionary<uint, IRedirect> Redirects = new();

    public void Add(IRedirect redirect) => Redirects.TryAdd(redirect.Id, redirect);

    public bool TryGetRemove(uint id, out IRedirect redirect) => Redirects.TryRemove(id, out redirect!);
}
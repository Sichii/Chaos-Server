using Chaos.Entities.Networking;
using Chaos.Networking.Interfaces;

namespace Chaos.Networking.Entities;

public class RedirectManager : IRedirectManager
{
    private readonly ConcurrentDictionary<uint, Redirect> Redirects = new();

    public void Add(Redirect redirect) => Redirects.TryAdd(redirect.Id, redirect);

    public bool TryGetRemove(uint id, out Redirect redirect) => Redirects.TryRemove(id, out redirect!);
}
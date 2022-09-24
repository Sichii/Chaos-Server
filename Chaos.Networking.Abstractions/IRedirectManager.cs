using Chaos.Entities.Networking;

namespace Chaos.Networking.Abstractions;

public interface IRedirectManager
{
    void Add(Redirect redirect);
    bool TryGetRemove(uint id, out Redirect redirect);
}
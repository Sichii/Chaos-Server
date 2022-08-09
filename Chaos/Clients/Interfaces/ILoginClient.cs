using Chaos.Data;
using Chaos.Networking.Definitions;
using Chaos.Networking.Interfaces;
using Chaos.Objects;
using Chaos.Services.Caches.Interfaces;

namespace Chaos.Clients.Interfaces;

public interface ILoginClient : ISocketClient
{
    void SendLoginControls(LoginControlsType loginControlsType, string message);
    void SendLoginMessage(LoginMessageType loginMessageType, string? message = null);
    void SendLoginNotice(bool full, Notice notice);
    void SendMetafile(MetafileRequestType metafileRequestType, ISimpleCache<Metafile> metafile, string? name = null);
}
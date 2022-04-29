using Chaos.Core.Definitions;
using Chaos.DataObjects;
using Chaos.Managers.Interfaces;
using Chaos.Networking.Interfaces;

namespace Chaos.Clients.Interfaces;

public interface ILoginClient : ISocketClient
{
    void SendLoginControls(LoginControlsType loginControlsType, string message);
    void SendLoginMessage(LoginMessageType loginMessageType, string? message = null);
    void SendLoginNotice(bool full, Notice notice);
    void SendMetafile(MetafileRequestType metafileRequestType, ICacheManager<string, Metafile> metafileManager, string? name = null);
}
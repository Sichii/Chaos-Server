using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Networking.Abstractions;
using Chaos.Objects;
using Chaos.Storage.Abstractions;

namespace Chaos.Clients.Abstractions;

public interface ILoginClient : ISocketClient
{
    void SendLoginControls(LoginControlsType loginControlsType, string message);
    void SendLoginMessage(LoginMessageType loginMessageType, string? message = null);
    void SendLoginNotice(bool full, Notice notice);
    void SendMetafile(MetafileRequestType metafileRequestType, ISimpleCache<Metafile> metafile, string? name = null);
}
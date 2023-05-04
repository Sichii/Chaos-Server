using Chaos.Common.Definitions;
using Chaos.Services.Storage.Abstractions;

namespace Chaos.Networking.Abstractions;

public interface ILoginClient : ISocketClient
{
    void SendLoginControls(LoginControlsType loginControlsType, string message);
    void SendLoginMessage(LoginMessageType loginMessageType, string? message = null);
    void SendLoginNotice(bool full, INotice notice);
    void SendMetafile(MetafileRequestType metafileRequestType, IMetaDataCache metaDataCache, string? name = null);
}
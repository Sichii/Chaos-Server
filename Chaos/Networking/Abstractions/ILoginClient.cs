using Chaos.Common.Definitions;
using Chaos.Services.Storage.Abstractions;

namespace Chaos.Networking.Abstractions;

public interface ILoginClient : ISocketClient
{
    void SendLoginControls(LoginControlsType loginControlsType, string message);
    void SendLoginMessage(LoginMessageType loginMessageType, string? message = null);
    void SendLoginNotice(bool full, INotice notice);
    void SendMetaData(MetaDataRequestType metaDataRequestType, IMetaDataStore metaDataStore, string? name = null);
}
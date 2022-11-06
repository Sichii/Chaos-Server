using Chaos.Clients.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets;

namespace Chaos.Servers.Abstractions;

public interface ILoginServer : IServer<ILoginClient>
{
    ValueTask OnClientRedirected(ILoginClient client, in ClientPacket packet);
    ValueTask OnCreateCharFinalize(ILoginClient client, in ClientPacket packet);
    ValueTask OnCreateCharRequest(ILoginClient client, in ClientPacket packet);
    ValueTask OnHomepageRequest(ILoginClient client, in ClientPacket packet);
    ValueTask OnLogin(ILoginClient client, in ClientPacket packet);
    ValueTask OnMetafileRequest(ILoginClient client, in ClientPacket packet);
    ValueTask OnNoticeRequest(ILoginClient client, in ClientPacket packet);
    ValueTask OnPasswordChange(ILoginClient client, in ClientPacket packet);
}
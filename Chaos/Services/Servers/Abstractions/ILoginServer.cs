using System.Threading.Tasks;
using Chaos.Clients.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets;

namespace Chaos.Services.Servers.Abstractions;

public interface ILoginServer : IServer<ILoginClient>
{
    ValueTask OnClientRedirected(ILoginClient client, ref ClientPacket packet);
    ValueTask OnCreateCharFinalize(ILoginClient client, ref ClientPacket packet);
    ValueTask OnCreateCharRequest(ILoginClient client, ref ClientPacket packet);
    ValueTask OnHomepageRequest(ILoginClient client, ref ClientPacket packet);
    ValueTask OnLogin(ILoginClient client, ref ClientPacket packet);
    ValueTask OnMetafileRequest(ILoginClient client, ref ClientPacket packet);
    ValueTask OnNoticeRequest(ILoginClient client, ref ClientPacket packet);
    ValueTask OnPasswordChange(ILoginClient client, ref ClientPacket packet);
}
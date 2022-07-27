using System.Threading.Tasks;
using Chaos.Clients.Interfaces;
using Chaos.Networking.Interfaces;
using Chaos.Packets;

namespace Chaos.Services.Hosted.Interfaces;

public interface ILoginServer : IServer
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
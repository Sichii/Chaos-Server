using System.Threading.Tasks;
using Chaos.Clients.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets;

namespace Chaos.Services.Hosted.Abstractions;

public interface ILobbyServer : IServer
{
    ValueTask OnConnectionInfoRequest(ILobbyClient client, ref ClientPacket packet);
    ValueTask OnServerTableRequest(ILobbyClient client, ref ClientPacket packet);
}
using System.Threading.Tasks;
using Chaos.Clients.Interfaces;
using Chaos.Networking.Interfaces;
using Chaos.Packets;

namespace Chaos.Servers.Interfaces;

public interface ILobbyServer : IServer
{
    ValueTask OnConnectionInfoRequest(ILobbyClient client, ref ClientPacket packet);
    ValueTask OnServerTableRequest(ILobbyClient client, ref ClientPacket packet);
}
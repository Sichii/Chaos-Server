using System.Threading.Tasks;
using Chaos.Clients.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets;

namespace Chaos.Services.Servers.Abstractions;

public interface ILobbyServer : IServer<ILobbyClient>
{
    ValueTask OnConnectionInfoRequest(ILobbyClient client, ref ClientPacket packet);
    ValueTask OnServerTableRequest(ILobbyClient client, ref ClientPacket packet);
}
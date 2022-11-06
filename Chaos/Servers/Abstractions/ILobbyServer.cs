using Chaos.Clients.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets;

namespace Chaos.Servers.Abstractions;

public interface ILobbyServer : IServer<ILobbyClient>
{
    ValueTask OnConnectionInfoRequest(ILobbyClient client, in ClientPacket packet);
    ValueTask OnServerTableRequest(ILobbyClient client, in ClientPacket packet);
}
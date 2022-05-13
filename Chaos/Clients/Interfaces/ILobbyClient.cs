using Chaos.Networking.Interfaces;
using Chaos.Objects;

namespace Chaos.Clients.Interfaces;

public interface ILobbyClient : ISocketClient
{
    void SendConnectionInfo(uint serverTableCheckSum);
    void SendServerTable(ServerTable serverTable);
}
using Chaos.Containers;
using Chaos.Networking.Interfaces;

namespace Chaos.Clients.Interfaces;

public interface ILobbyClient : ISocketClient
{
    void SendConnectionInfo(uint serverTableCheckSum);
    void SendServerTable(ServerTable serverTable);
}
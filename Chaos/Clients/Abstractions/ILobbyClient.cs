using Chaos.Networking.Abstractions;
using Chaos.Objects;

namespace Chaos.Clients.Abstractions;

public interface ILobbyClient : ISocketClient
{
    void SendConnectionInfo(uint serverTableCheckSum);
    void SendServerTable(ServerTable serverTable);
}
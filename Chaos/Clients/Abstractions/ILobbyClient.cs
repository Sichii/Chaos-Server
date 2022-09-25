using Chaos.Networking.Abstractions;

namespace Chaos.Clients.Abstractions;

public interface ILobbyClient : ISocketClient
{
    void SendConnectionInfo(uint serverTableCheckSum);
    void SendServerTable(IServerTable serverTable);
}
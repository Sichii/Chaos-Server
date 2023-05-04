namespace Chaos.Networking.Abstractions;

public interface ILobbyClient : ISocketClient
{
    void SendConnectionInfo(uint serverTableCheckSum);
    void SendServerTable(IServerTable serverTable);
}
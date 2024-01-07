namespace Chaos.Networking.Abstractions;

public interface ILobbyClient : IServerClient
{
    void SendConnectionInfo(uint serverTableCheckSum);
    void SendServerTable(IServerTable serverTable);
}
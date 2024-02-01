namespace Chaos.Networking.Abstractions;

public interface ILobbyClient : IConnectedClient
{
    void SendConnectionInfo(uint serverTableCheckSum);
    void SendServerTableResponse(IServerTable serverTable);
}
namespace Chaos.Networking.Abstractions;

public interface IChaosLobbyClient : IConnectedClient
{
    void SendConnectionInfo(uint serverTableCheckSum);
    void SendServerTableResponse(IServerTable serverTable);
}
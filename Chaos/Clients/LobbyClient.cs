using System.Net.Sockets;
using Chaos.Clients.Interfaces;
using Chaos.Cryptography.Interfaces;
using Chaos.Entities.Networking.Server;
using Chaos.Networking.Abstractions;
using Chaos.Objects;
using Chaos.Packets.Interfaces;
using Chaos.Services.Hosted.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Clients;

public class LobbyClient : SocketClientBase, ILobbyClient
{
    public LobbyClient(
        Socket socket,
        ICryptoClient cryptoClient,
        ILobbyServer server,
        IPacketSerializer packetSerializer,
        ILogger<LobbyClient> logger
    )
        : base(
            socket,
            cryptoClient,
            server,
            packetSerializer,
            logger) { }

    public void SendConnectionInfo(uint serverTableCheckSum)
    {
        var args = new ConnectionInfoArgs
        {
            Key = CryptoClient.Key,
            Seed = CryptoClient.Seed,
            TableCheckSum = serverTableCheckSum
        };

        Send(args);
    }

    public void SendServerTable(ServerTable serverTable)
    {
        var args = new ServerTableArgs
        {
            ServerTable = serverTable.Data
        };

        Send(args);
    }
}
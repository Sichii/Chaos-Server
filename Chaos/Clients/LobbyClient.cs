using System.Net.Sockets;
using System.Threading.Tasks;
using Chaos.Clients.Abstractions;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Services.Servers.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Clients;

public sealed class LobbyClient : SocketClientBase, ILobbyClient
{
    private readonly ILobbyServer Server;

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
            packetSerializer,
            logger) => Server = server;

    /// <inheritdoc />
    protected override ValueTask HandlePacketAsync(Span<byte> span)
    {
        var opCode = span[3];
        var isEncrypted = CryptoClient.ShouldBeEncrypted(opCode);
        var packet = new ClientPacket(ref span, isEncrypted);

        if (isEncrypted)
            CryptoClient.Decrypt(ref packet);

        Logger.LogTrace("[Rcv] {Packet}", packet.ToString());

        return Server.HandlePacketAsync(this, ref packet);
    }

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

    public void SendServerTable(IServerTable serverTable)
    {
        var args = new ServerTableArgs
        {
            ServerTable = serverTable.Data
        };

        Send(args);
    }
}
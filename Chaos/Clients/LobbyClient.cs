using System.Net.Sockets;
using Chaos.Clients.Abstractions;
using Chaos.Cryptography.Abstractions;
using Chaos.Extensions.Networking;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Clients;

public sealed class LobbyClient : SocketClientBase, ILobbyClient
{
    private readonly ILobbyServer<ILobbyClient> Server;

    public LobbyClient(
        Socket socket,
        IOptions<ChaosOptions> chaosOptions,
        ICrypto crypto,
        ILobbyServer<ILobbyClient> server,
        IPacketSerializer packetSerializer,
        ILogger<LobbyClient> logger
    )
        : base(
            socket,
            crypto,
            packetSerializer,
            logger)
    {
        LogRawPackets = chaosOptions.Value.LogRawPackets;
        Server = server;
    }

    /// <inheritdoc />
    protected override ValueTask HandlePacketAsync(Span<byte> span)
    {
        var opCode = span[3];
        var isEncrypted = Crypto.ShouldBeEncrypted(opCode);
        var packet = new ClientPacket(ref span, isEncrypted);

        if (isEncrypted)
            Crypto.Decrypt(ref packet);

        if (LogRawPackets)
            Logger.LogTrace("[Rcv] {Packet}", packet.ToString());

        return Server.HandlePacketAsync(this, in packet);
    }

    public void SendConnectionInfo(uint serverTableCheckSum)
    {
        var args = new ConnectionInfoArgs
        {
            Key = Crypto.Key,
            Seed = Crypto.Seed,
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
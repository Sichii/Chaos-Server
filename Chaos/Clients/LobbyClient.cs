using System.Net.Sockets;
using Chaos.Clients.Abstractions;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Clients;

public sealed class LobbyClient : SocketClientBase, ILobbyClient
{
    private readonly ILobbyServer<ILobbyClient> Server;

    public LobbyClient(
        Socket socket,
        ICryptoClient cryptoClient,
        ILobbyServer<ILobbyClient> server,
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

        //no way to pass the packet in because its a ref struct
        //but we still want to avoid serializing the packet to a string if we aren't actually going to log it
        if (Logger.IsEnabled(LogLevel.Trace))
            Logger.LogTrace("[Rcv] {Packet}", packet.ToString());

        return Server.HandlePacketAsync(this, in packet);
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
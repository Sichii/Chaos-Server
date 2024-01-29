using System.Net.Sockets;
using Chaos.Cryptography.Abstractions;
using Chaos.Extensions.Networking;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Options;

namespace Chaos.Networking;

public sealed class LobbyClient : ConnectedClientBase, ILobbyClient
{
    private readonly ILobbyServer<ILobbyClient> Server;

    public LobbyClient(
        Socket socket,
        IOptions<ChaosOptions> chaosOptions,
        ICrypto crypto,
        ILobbyServer<ILobbyClient> server,
        IPacketSerializer packetSerializer,
        ILogger<LobbyClient> logger)
        : base(
            socket,
            crypto,
            packetSerializer,
            logger)
    {
        LogRawPackets = chaosOptions.Value.LogRawPackets;
        Server = server;
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

    /// <inheritdoc />
    protected override ValueTask HandlePacketAsync(Span<byte> span)
    {
        var opCode = span[3];
        var packet = new Packet(ref span, Crypto.IsClientEncrypted(opCode));

        if (packet.IsEncrypted)
            Crypto.Decrypt(ref packet);

        if (LogRawPackets)
            Logger.WithTopics(
                      Topics.Servers.LobbyServer,
                      Topics.Qualifiers.Raw,
                      Topics.Entities.Packet,
                      Topics.Actions.Receive)
                  .WithProperty(this)
                  .LogTrace("[Rcv] {@Packet}", packet.ToString());

        return Server.HandlePacketAsync(this, in packet);
    }
}
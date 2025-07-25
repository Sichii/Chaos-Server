#region
using System.Net.Sockets;
using System.Text;
using Chaos.Cryptography.Abstractions;
using Chaos.Extensions.Networking;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Networking;

public sealed class ChaosLobbyClient : LobbyClientBase, IChaosLobbyClient
{
    private readonly ILobbyServer<IChaosLobbyClient> Server;

    public ChaosLobbyClient(
        Socket socket,
        IOptions<ChaosOptions> chaosOptions,
        ICrypto crypto,
        ILobbyServer<IChaosLobbyClient> server,
        IPacketSerializer packetSerializer,
        ILogger<ChaosLobbyClient> logger)
        : base(
            socket,
            crypto,
            packetSerializer,
            logger)
    {
        LogRawPackets = chaosOptions.Value.LogRawPackets;
        LogSendPacketCode = chaosOptions.Value.LogSendPacketCode;
        LogReceivePacketCode = chaosOptions.Value.LogReceivePacketCode;
        Server = server;
    }

    public void SendConnectionInfo(uint serverTableCheckSum)
    {
        Crypto.GenerateEncryptionParameters();

        var args = new ConnectionInfoArgs
        {
            Key = Encoding.ASCII.GetString(Crypto.Key),
            Seed = Crypto.Seed,
            TableCheckSum = serverTableCheckSum
        };

        Send(args);
    }

    public void SendServerTableResponse(IServerTable serverTable)
    {
        var args = new ServerTableResponseArgs
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
        else if (LogReceivePacketCode)
            Logger.WithTopics(
                      Topics.Qualifiers.Raw,
                      Topics.Entities.Client,
                      Topics.Entities.Packet,
                      Topics.Actions.Receive)
                  .WithProperty(this)
                  .LogTrace("Received packet with code {@OpCode} from {@ClientIp}", opCode, RemoteIp);

        return Server.HandlePacketAsync(this, in packet);
    }
}
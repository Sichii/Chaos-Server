#region
using System.Net.Sockets;
using Chaos.Cryptography.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Networking;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Networking;

public sealed class ChaosLoginClient : LoginClientBase, IChaosLoginClient
{
    private readonly ITypeMapper Mapper;
    private readonly ILoginServer<IChaosLoginClient> Server;

    public ChaosLoginClient(
        Socket socket,
        IOptions<ChaosOptions> chaosOptions,
        ICrypto crypto,
        ILoginServer<IChaosLoginClient> server,
        IPacketSerializer packetSerializer,
        ILogger<ChaosLoginClient> logger,
        ITypeMapper mapper)
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
        Mapper = mapper;
    }

    public void SendLoginControl(LoginControlsType loginControlsType, string message)
    {
        var args = new LoginControlArgs
        {
            LoginControlsType = loginControlsType,
            Message = message
        };

        Send(args);
    }

    public void SendLoginMessage(LoginMessageType loginMessageType, string? message = null)
    {
        var args = new LoginMessageArgs
        {
            LoginMessageType = loginMessageType,
            Message = message
        };

        Send(args);
    }

    public void SendLoginNotice(bool full, INotice notice)
    {
        var args = new LoginNoticeArgs
        {
            IsFullResponse = full
        };

        if (full)
            args.Data = notice.Data;
        else
            args.CheckSum = notice.CheckSum;

        Send(args);
    }

    public void SendMetaData(MetaDataRequestType metaDataRequestType, IMetaDataStore metaDataStore, string? name = null)
    {
        var args = new MetaDataArgs
        {
            MetaDataRequestType = metaDataRequestType
        };

        switch (metaDataRequestType)
        {
            case MetaDataRequestType.DataByName:
            {
                ArgumentNullException.ThrowIfNull(name);

                var metadata = metaDataStore.Get(name);

                args.MetaDataInfo = Mapper.Map<MetaDataInfo>(metadata);

                break;
            }
            case MetaDataRequestType.AllCheckSums:
            {
                args.MetaDataCollection = Mapper.MapMany<MetaDataInfo>(metaDataStore)
                                                .ToList();

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(metaDataRequestType), metaDataRequestType, "Unknown enum value");
        }

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
                      Topics.Servers.LoginServer,
                      Topics.Qualifiers.Raw,
                      Topics.Entities.Client,
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
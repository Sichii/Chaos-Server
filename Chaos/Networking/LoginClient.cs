using System.Net.Sockets;
using Chaos.Common.Definitions;
using Chaos.Cryptography.Abstractions;
using Chaos.Extensions;
using Chaos.Extensions.Networking;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Networking;

public sealed class LoginClient : SocketClientBase, ILoginClient
{
    private readonly ITypeMapper Mapper;
    private readonly ILoginServer<ILoginClient> Server;

    public LoginClient(
        Socket socket,
        IOptions<ChaosOptions> chaosOptions,
        ICrypto crypto,
        ILoginServer<ILoginClient> server,
        IPacketSerializer packetSerializer,
        ILogger<LoginClient> logger,
        ITypeMapper mapper
    )
        : base(
            socket,
            crypto,
            packetSerializer,
            logger)
    {
        LogRawPackets = chaosOptions.Value.LogRawPackets;
        Server = server;
        Mapper = mapper;
    }

    public void SendLoginControls(LoginControlsType loginControlsType, string message)
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
        var isEncrypted = Crypto.ShouldBeEncrypted(opCode);
        var packet = new ClientPacket(ref span, isEncrypted);

        if (isEncrypted)
            Crypto.Decrypt(ref packet);

        if (LogRawPackets)
            Logger.WithProperty(this)
                  .LogTrace("[Rcv] {@Packet}", packet.ToString());

        return Server.HandlePacketAsync(this, in packet);
    }
}
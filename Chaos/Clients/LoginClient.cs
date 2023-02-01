using System.Net.Sockets;
using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Clients;

public sealed class LoginClient : SocketClientBase, ILoginClient
{
    private readonly ILoginServer<ILoginClient> Server;

    public LoginClient(
        Socket socket,
        IOptions<ChaosOptions> chaosOptions,
        ICryptoClient cryptoClient,
        ILoginServer<ILoginClient> server,
        IPacketSerializer packetSerializer,
        ILogger<LoginClient> logger
    )
        : base(
            socket,
            cryptoClient,
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
        var isEncrypted = CryptoClient.ShouldBeEncrypted(opCode);
        var packet = new ClientPacket(ref span, isEncrypted);

        if (isEncrypted)
            CryptoClient.Decrypt(ref packet);

        if (LogRawPackets)
            Logger.LogTrace("[Rcv] {Packet}", packet.ToString());

        return Server.HandlePacketAsync(this, in packet);
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
        var args = new NoticeRequestArgs
        {
            IsFullResponse = full
        };

        if (full)
            args.Notification = notice.Data;
        else
            args.NotificationCheckSum = notice.CheckSum;

        Send(args);
    }

    public void SendMetafile(MetafileRequestType metafileRequestType, IMetaDataCache metaDataCache, string? name = null)
    {
        var args = new MetafileArgs
        {
            MetafileRequestType = metafileRequestType
        };

        switch (metafileRequestType)
        {
            case MetafileRequestType.DataByName:
            {
                ArgumentNullException.ThrowIfNull(name);

                var metafile = metaDataCache.GetMetafile(name);

                args.MetafileData = new MetafileInfo
                {
                    Name = metafile.Name,
                    CheckSum = metafile.CheckSum,
                    Data = metafile.Data
                };

                break;
            }
            case MetafileRequestType.AllCheckSums:
            {
                args.Info = metaDataCache.Select(
                                             metafile => new MetafileInfo
                                             {
                                                 Name = metafile.Name,
                                                 CheckSum = metafile.CheckSum
                                             })
                                         .ToList();

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(metafileRequestType), metafileRequestType, "Unknown enum value");
        }

        Send(args);
    }
}
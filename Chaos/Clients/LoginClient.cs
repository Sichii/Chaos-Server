using System.Net.Sockets;
using Chaos.Clients.Interfaces;
using Chaos.Cryptography.Interfaces;
using Chaos.Data;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Interfaces;
using Chaos.Networking.Model.Server;
using Chaos.Objects;
using Chaos.Packets.Interfaces;
using Chaos.Services.Caches.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Clients;

public class LoginClient : SocketClientBase, ILoginClient
{
    public LoginClient(
        Socket socket,
        ICryptoClient cryptoClient,
        IServer server,
        IPacketSerializer packetSerializer,
        ILogger<LoginClient> logger
    )
        : base(
            socket,
            cryptoClient,
            server,
            packetSerializer,
            logger) { }

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

    public void SendLoginNotice(bool full, Notice notice)
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

    public void SendMetafile(MetafileRequestType metafileRequestType, ISimpleCache<Metafile> metafileCache, string? name = null)
    {
        var args = new MetafileArgs
        {
            MetafileRequestType = metafileRequestType
        };

        switch (metafileRequestType)
        {
            case MetafileRequestType.DataByName:
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                var metafile = metafileCache.GetObject(name);

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
                args.Info = metafileCache.Select(
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
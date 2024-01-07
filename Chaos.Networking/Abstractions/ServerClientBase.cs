using System.Net.Sockets;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Provides the ability to send and receive packets to and from a client over a socket, from a server
/// </summary>
public abstract class ServerClientBase : SocketClientBase, IServerClient
{
    /// <inheritdoc />
    protected ServerClientBase(
        Socket socket,
        ICrypto crypto,
        IPacketSerializer packetSerializer,
        ILogger<ServerClientBase> logger)
        : base(
            socket,
            crypto,
            packetSerializer,
            logger) { }

    /// <inheritdoc />
    public virtual void SendAcceptConnection(string message)
    {
        var args = new AcceptConnectionArgs
        {
            Message = message
        };

        Send(args);
    }

    /// <inheritdoc />
    public virtual void SendHeartBeat(byte first, byte second)
    {
        var args = new HeartBeatResponseArgs
        {
            First = first,
            Second = second
        };

        Send(args);
    }

    /// <inheritdoc />
    public virtual void SendRedirect(IRedirect redirect)
    {
        var args = new RedirectArgs
        {
            EndPoint = redirect.EndPoint,
            Seed = redirect.Seed,
            Key = redirect.Key,
            Name = redirect.Name,
            Id = redirect.Id
        };

        Send(args);
    }
}
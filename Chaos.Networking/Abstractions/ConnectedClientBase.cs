using System.Net.Sockets;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Represents a client that is connected to an <see cref="IServer{T}" />. This class defines the methods used to
///     communicate with the client.
/// </summary>
public abstract class ConnectedClientBase : SocketClientBase, IConnectedClient
{
    /// <inheritdoc />
    protected ConnectedClientBase(
        Socket socket,
        ICrypto crypto,
        IPacketSerializer packetSerializer,
        ILogger<ConnectedClientBase> logger)
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

    /// <inheritdoc />
    public override void Encrypt(ref Packet packet) => Crypto.ServerEncrypt(ref packet.Buffer, packet.OpCode, packet.Sequence);

    /// <inheritdoc />
    public override bool IsEncrypted(byte opCode) => Crypto.IsServerEncrypted(opCode);
}
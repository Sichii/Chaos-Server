using System.Net.Sockets;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Represents a client connected to a login server.
/// </summary>
public abstract class LoginClientBase : ConnectedClientBase, ILoginClient
{
    /// <inheritdoc />
    protected LoginClientBase(
        Socket socket,
        ICrypto crypto,
        IPacketSerializer packetSerializer,
        ILogger<LoginClientBase> logger)
        : base(
            socket,
            crypto,
            packetSerializer,
            logger) { }

    /// <inheritdoc />
    public virtual void SendLoginControl(LoginControlArgs args) => Send(args);

    /// <inheritdoc />
    public virtual void SendLoginMessage(LoginMessageArgs args) => Send(args);

    /// <inheritdoc />
    public virtual void SendLoginNotice(LoginNoticeArgs args) => Send(args);

    /// <inheritdoc />
    public virtual void SendMetaData(MetaDataArgs args) => Send(args);
}
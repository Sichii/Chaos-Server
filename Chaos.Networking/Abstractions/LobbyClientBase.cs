using System.Net.Sockets;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Represents a client connected to a lobby server.
/// </summary>
public abstract class LobbyClientBase : ConnectedClientBase, ILobbyClient
{
    /// <inheritdoc />
    protected LobbyClientBase(
        Socket socket,
        ICrypto crypto,
        IPacketSerializer packetSerializer,
        ILogger<LobbyClientBase> logger)
        : base(
            socket,
            crypto,
            packetSerializer,
            logger) { }

    /// <inheritdoc />
    public virtual void SendConnectionInfo(ConnectionInfoArgs args) => Send(args);

    /// <inheritdoc />
    public virtual void SendServerTableResponse(ServerTableResponseArgs args) => Send(args);
}
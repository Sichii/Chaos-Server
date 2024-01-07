using Chaos.Packets;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines a pattern for server that presents a list of available login servers to connect to
/// </summary>
public interface ILobbyServer<in TClient> : IServer<TClient> where TClient: IServerClient
{
    /// <summary>
    ///     Occurs when a client requests the details of available login servers
    /// </summary>
    ValueTask OnServerTableRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests the encryption details, and a checksum of the details of available login servers
    /// </summary>
    ValueTask OnVersion(TClient client, in Packet packet);
}
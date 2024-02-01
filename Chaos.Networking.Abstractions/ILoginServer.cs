using Chaos.Packets;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines a pattern for a server that facilitates character creation, presentation of the EULA, and the ability to
///     log into a character or change a character's password
/// </summary>
public interface ILoginServer<in TClient> : IServer<TClient> where TClient: IConnectedClient
{
    /// <summary>
    ///     Occurs when a client is redirected to this login server. This redirect could have come from either a lobby server
    ///     or a world server
    /// </summary>
    ValueTask OnClientRedirected(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests to create a new character. This is the second step in the process and will contain
    ///     appearance details
    /// </summary>
    ValueTask OnCreateCharFinalize(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests to create a new character. This is the first step in the process and will only
    ///     contain a name and password
    /// </summary>
    ValueTask OnCreateCharInitial(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests the url of the homepage
    /// </summary>
    ValueTask OnHomepageRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client provides credentials to log into the world
    /// </summary>
    ValueTask OnLogin(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests either a metadata hash, or a full copy of the metadata
    /// </summary>
    ValueTask OnMetaDataRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests EULA details
    /// </summary>
    ValueTask OnNoticeRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests to change a character's password
    /// </summary>
    ValueTask OnPasswordChange(TClient client, in Packet packet);
}
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Represents a client that is connected to the login server.
/// </summary>
public interface ILoginClient : IConnectedClient
{
    /// <summary>
    ///     Sends a login control message to the client.
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.LoginControl" />
    /// </remarks>
    void SendLoginControl(LoginControlArgs args);

    /// <summary>
    ///     Sends a login message to the client.
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.LoginMessage" />
    /// </remarks>
    void SendLoginMessage(LoginMessageArgs args);

    /// <summary>
    ///     Sends a login notice to the client.
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.LoginNotice" />
    /// </remarks>
    void SendLoginNotice(LoginNoticeArgs args);

    /// <summary>
    ///     Sends meta data to the client.
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.MetaData" />
    /// </remarks>
    void SendMetaData(MetaDataArgs args);
}
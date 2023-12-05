using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.ServerMessage" /> packet
/// </summary>
public sealed record ServerMessageArgs : ISendArgs
{
    /// <summary>
    ///     The message to display
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    ///     The type of message to display
    /// </summary>
    public ServerMessageType ServerMessageType { get; set; }
}
using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.ServerMessage" /> packet
/// </summary>
public sealed record ServerMessageArgs : IPacketSerializable
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
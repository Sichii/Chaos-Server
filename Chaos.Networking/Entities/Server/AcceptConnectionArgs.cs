using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.AcceptConnection" /> packet
/// </summary>
public sealed record AcceptConnectionArgs : IPacketSerializable
{
    /// <summary>
    ///     The message to send to the client. In official DA, this is "CONNECTED SERVER", but it doesnt really matter what you
    ///     send
    /// </summary>
    public required string Message { get; set; }
}
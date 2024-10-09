using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.Turn" /> packet
/// </summary>
public sealed record TurnArgs : IPacketSerializable
{
    /// <summary>
    ///     The direction the client is trying to turn
    /// </summary>
    public required Direction Direction { get; set; }
}
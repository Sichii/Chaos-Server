using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.Location" />
///     packet
/// </summary>
public sealed record LocationArgs : IPacketSerializable
{
    /// <summary>
    ///     The X coordinate of the player
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///     The Y coordinate of the player
    /// </summary>
    public int Y { get; set; }
}
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.ServerTable" />
///     packet
/// </summary>
public sealed record ServerTableArgs : IPacketSerializable
{
    /// <summary>
    ///     The raw server table data
    /// </summary>
    public byte[] ServerTable { get; set; } = null!;
}
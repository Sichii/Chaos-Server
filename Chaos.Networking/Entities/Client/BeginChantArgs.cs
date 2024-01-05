using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.BeginChant" />
///     packet
/// </summary>
public sealed record BeginChantArgs : IPacketSerializable
{
    /// <summary>
    ///     The number of cast lines for the spell being chanted
    /// </summary>
    public required byte CastLineCount { get; set; }
}
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.BeginChant" /> packet
/// </summary>
public sealed record BeginChantArgs : IPacketSerializable
{
    /// <summary>
    ///     The number of cast lines for the spell being chanted
    /// </summary>
    public required byte CastLineCount { get; set; }
}
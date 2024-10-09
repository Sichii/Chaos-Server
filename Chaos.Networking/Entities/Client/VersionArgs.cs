using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.Version" /> packet
/// </summary>
public sealed record VersionArgs : IPacketSerializable
{
    /// <summary>
    ///     The client version as a single number
    /// </summary>
    public required ushort Version { get; set; }
}
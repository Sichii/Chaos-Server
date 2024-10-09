using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.HeartBeat" /> packet
/// </summary>
public sealed record HeartBeatArgs : IPacketSerializable
{
    /// <summary>
    ///     The first byte (the client expects these bytes in reverse for it's response)
    /// </summary>
    public required byte First { get; set; }

    /// <summary>
    ///     The second byte (the client expects these bytes in reverse for it's response)
    /// </summary>
    public required byte Second { get; set; }
}
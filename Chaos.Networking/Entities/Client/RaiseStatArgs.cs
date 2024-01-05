using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.RaiseStat" />
///     packet
/// </summary>
public sealed record RaiseStatArgs : IPacketSerializable
{
    /// <summary>
    ///     The stat the client is trying to raise
    /// </summary>
    public required Stat Stat { get; set; }
}
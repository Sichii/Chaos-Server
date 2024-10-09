using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.RaiseStat" /> packet
/// </summary>
public sealed record RaiseStatArgs : IPacketSerializable
{
    /// <summary>
    ///     The stat the client is trying to raise
    /// </summary>
    public required Stat Stat { get; set; }
}
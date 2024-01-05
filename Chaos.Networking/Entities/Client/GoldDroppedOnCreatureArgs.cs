using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.GoldDroppedOnCreature" /> packet
/// </summary>
public sealed record GoldDroppedOnCreatureArgs : IPacketSerializable
{
    /// <summary>
    ///     The amount of gold the client is trying to drop on the creature
    /// </summary>
    public required int Amount { get; set; }

    /// <summary>
    ///     The id of the creature the client is trying to drop gold on
    /// </summary>
    public required uint TargetId { get; set; }
}
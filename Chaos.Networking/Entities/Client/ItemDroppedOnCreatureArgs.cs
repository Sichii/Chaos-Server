using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.ItemDroppedOnCreature" /> packet
///     <br />
/// </summary>
public sealed record ItemDroppedOnCreatureArgs : IPacketSerializable
{
    /// <summary>
    ///     The amount of the item the client is trying to drop on the creature
    /// </summary>
    public required byte Count { get; set; }

    /// <summary>
    ///     The slot of the item the client is trying to drop on the creature
    /// </summary>
    public required byte SourceSlot { get; set; }

    /// <summary>
    ///     The id of the creature the client is trying to drop the item on
    /// </summary>
    public required uint TargetId { get; set; }
}
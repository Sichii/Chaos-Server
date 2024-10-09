using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.ExchangeInteraction" /> packet
/// </summary>
public sealed record ExchangeInteractionArgs : IPacketSerializable
{
    /// <summary>
    ///     The type of action being requested
    /// </summary>
    public required ExchangeRequestType ExchangeRequestType { get; set; }

    /// <summary>
    ///     If specified, the amount of gold being set in the exchange (set, not added!)
    /// </summary>
    public int? GoldAmount { get; set; }

    /// <summary>
    ///     If specified, the count of the item being added
    /// </summary>
    public byte? ItemCount { get; set; }

    /// <summary>
    ///     The id of the player on the other side of the exchange
    /// </summary>
    public uint OtherPlayerId { get; set; }

    /// <summary>
    ///     If specified, the slot of the item being added
    /// </summary>
    public byte? SourceSlot { get; set; }
}
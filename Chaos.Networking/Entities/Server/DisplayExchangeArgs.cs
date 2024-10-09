using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.DisplayExchange" /> packet
/// </summary>
public sealed record DisplayExchangeArgs : IPacketSerializable
{
    /// <summary>
    ///     The index in the exchange window of the item being added (or updated)
    /// </summary>
    public byte? ExchangeIndex { get; set; }

    /// <summary>
    ///     The type of action being performed on the exchange window
    /// </summary>
    public ExchangeResponseType ExchangeResponseType { get; set; }

    /// <summary>
    ///     If trying to add a stackable item to the exchange, this is the slot that item is in for the purposes of requesting
    ///     a stack amount of that item
    /// </summary>
    public byte? FromSlot { get; set; }

    /// <summary>
    ///     If trying to set a gold value in the exchange, this is the amount of gold being set (SET, NOT ADDED)
    /// </summary>
    public int? GoldAmount { get; set; }

    /// <summary>
    ///     If trying to add an item to the exchange, this is the color of that item
    /// </summary>
    public DisplayColor? ItemColor { get; set; }

    /// <summary>
    ///     If trying to add an item to the exchange, this is the name of that item
    /// </summary>
    public string? ItemName { get; set; }

    /// <summary>
    ///     If trying to add an item to the exchange, this is the sprite of that item
    /// </summary>
    public ushort? ItemSprite { get; set; }

    /// <summary>
    ///     When the exchange is accepted or canceled, this is the message that will display
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    ///     If trying to start an exchange, this is the id of the other aisling you are trying to start an exchange with
    /// </summary>
    public uint? OtherUserId { get; set; }

    /// <summary>
    ///     If trying to start an exchange, this is the name of the other aisling you are trying to start an exchange with
    /// </summary>
    public string OtherUserName { get; set; } = null!;

    /// <summary>
    ///     If accepting an exchange, this value should be true if both parties have not accepted. If both parties have
    ///     accepted, this value should be false
    /// </summary>
    public bool? PersistExchange { get; set; }

    /// <summary>
    ///     Whether or not this packet is interacting with the right side of the exchange window. Keep in mind you will
    ///     interact with the right side when sending the results of one players actions on their side (the left) to the other
    ///     player. Each player will see the other player's actions occur on the right side, and their own actions on their
    ///     left side.
    /// </summary>
    public bool? RightSide { get; set; }
}
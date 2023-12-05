using Chaos.Common.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of an item used by several packets
/// </summary>
public sealed record ItemInfo
{
    /// <summary>
    ///     The color of the item
    /// </summary>
    public required DisplayColor Color { get; init; }

    /// <summary>
    ///     The cost of the item
    /// </summary>
    public required int? Cost { get; init; }

    /// <summary>
    ///     The count of the item
    /// </summary>
    public required uint? Count { get; init; }

    /// <summary>
    ///     The current durability of the item
    /// </summary>
    public required int CurrentDurability { get; init; }

    /// <summary>
    ///     The maximum durability of the item
    /// </summary>
    public required int MaxDurability { get; init; }

    /// <summary>
    ///     The name of the item
    /// </summary>
    public required string Name { get; init; } = null!;

    /// <summary>
    ///     The slot of the item
    /// </summary>
    public required byte Slot { get; init; }

    /// <summary>
    ///     The sprite of the item
    /// </summary>
    public required ushort Sprite { get; init; }

    /// <summary>
    ///     Whether or not the item is stackable
    /// </summary>
    public required bool Stackable { get; init; }
}
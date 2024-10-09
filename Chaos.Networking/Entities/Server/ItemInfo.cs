using Chaos.DarkAges.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of an item used by several packets
/// </summary>
public sealed record ItemInfo
{
    /// <summary>
    ///     The color of the item
    /// </summary>
    public DisplayColor Color { get; init; }

    /// <summary>
    ///     The cost of the item
    /// </summary>
    public int? Cost { get; init; }

    /// <summary>
    ///     The count of the item
    /// </summary>
    public uint? Count { get; init; }

    /// <summary>
    ///     The current durability of the item
    /// </summary>
    public int CurrentDurability { get; init; }

    /// <summary>
    ///     The maximum durability of the item
    /// </summary>
    public int MaxDurability { get; init; }

    /// <summary>
    ///     The name of the item
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    ///     The slot of the item
    /// </summary>
    public byte Slot { get; init; }

    /// <summary>
    ///     The sprite of the item
    /// </summary>
    public ushort Sprite { get; init; }

    /// <summary>
    ///     Whether or not the item is stackable
    /// </summary>
    public bool Stackable { get; init; }
}
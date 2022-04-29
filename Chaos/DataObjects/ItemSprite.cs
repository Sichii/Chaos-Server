using System.Text.Json.Serialization;
using Chaos.Core.Definitions;

namespace Chaos.DataObjects;

public record ItemSprite
{
    /// <summary>
    ///     The sprite number when the item is equipped.
    /// </summary>
    public ushort DisplaySprite { get; init; }
    /// <summary>
    ///     The raw sprite number without an offset.
    /// </summary>
    [JsonIgnore]
    public ushort RawSprite { get; init; }
    /// <summary>
    ///     The sprite number when the item is in the inventory.
    /// </summary>
    public ushort Sprite { get; init; }

    public ItemSprite(ushort sprite, ushort displaySprite)
    {
        Sprite = sprite;
        DisplaySprite = displaySprite;
        RawSprite = (ushort)(sprite + CONSTANTS.ITEM_SPRITE_OFFSET);
    }
}
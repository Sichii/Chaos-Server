using System.Text.Json.Serialization;
using Chaos.Core.Definitions;

namespace Chaos.Core.Data;

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
    public ushort OffsetPanelSprite { get; init; }
    /// <summary>
    ///     The sprite number when the item is in the inventory.
    /// </summary>
    public ushort PanelSprite { get; init; }

    [JsonConstructor]
    public ItemSprite(ushort panelSprite, ushort displaySprite)
    {
        PanelSprite = panelSprite;
        DisplaySprite = displaySprite;
        OffsetPanelSprite = (ushort)(panelSprite + CONSTANTS.ITEM_SPRITE_OFFSET);
    }
}
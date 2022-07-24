using System.Text.Json.Serialization;

namespace Chaos.Data;

public record ItemSprite
{
    /// <summary>
    ///     The sprite number for displaying the item on your character when the item is equipped.
    /// </summary>
    public ushort DisplaySprite { get; init; }
    /// <summary>
    ///     The sprite number with the positive offset. Used for items on the ground.
    /// </summary>
    [JsonIgnore]
    public ushort OffsetPanelSprite { get; init; }
    /// <summary>
    ///     The sprite number for displaying the item on the UI. (Inventory and Equipment panels)
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
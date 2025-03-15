namespace Chaos.Models.Data;

public sealed record ItemSprite(ushort PanelSprite, ushort DisplaySprite)
{
    /// <summary>
    ///     The sprite number for displaying the item on your character when the item is equipped.
    /// </summary>
    public ushort DisplaySprite { get; set; } = DisplaySprite;

    /// <summary>
    ///     The sprite number for displaying the item on the UI. (Inventory and Equipment panels)
    /// </summary>
    public ushort PanelSprite { get; set; } = PanelSprite;
}
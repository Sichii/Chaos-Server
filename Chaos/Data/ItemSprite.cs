namespace Chaos.Data;

public sealed record ItemSprite(ushort PanelSprite, ushort DisplaySprite)
{
    /// <summary>
    ///     The sprite number for displaying the item on your character when the item is equipped.
    /// </summary>
    public ushort DisplaySprite { get; init; } = DisplaySprite;

    /// <summary>
    ///     The sprite number for displaying the item on the UI. (Inventory and Equipment panels)
    /// </summary>
    public ushort PanelSprite { get; init; } = PanelSprite;
}
using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

public class GroundItem : NamedObject
{
    public Item Item { get; }

    public GroundItem(Item item)
        : base(
            item.DisplayName,
            default!,
            default,
            item.Template.ItemSprite.OffsetPanelSprite) => Item = item;

    public GroundItem(Item item, MapInstance mapInstance, Point point)
        : base(
            item.DisplayName,
            mapInstance,
            point,
            item.Template.ItemSprite.OffsetPanelSprite) => Item = item;

    public override void OnClicked(User source)
    {
        //nothing
        //there's a different packet for picking up items
    }
}
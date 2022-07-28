using Chaos.Containers;
using Chaos.Geometry.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

public class GroundItem : NamedEntity
{
    public Item Item { get; }

    public GroundItem(Item item, MapInstance mapInstance, IPoint point)
        : base(
            item.DisplayName,
            item.Template.ItemSprite.OffsetPanelSprite,
            mapInstance,
            point) =>
        Item = item;

    public override void OnClicked(Aisling source)
    {
        //nothing
        //there's a different packet for picking up items
    }
}
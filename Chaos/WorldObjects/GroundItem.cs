using Chaos.Containers;
using Chaos.Core.Geometry;
using Chaos.PanelObjects;
using Chaos.WorldObjects.Abstractions;

namespace Chaos.WorldObjects;

public class GroundItem : NamedObject
{
    public Item Item { get; }

    public GroundItem(Item item)
        : base(item.DisplayName, default!, default, item.Template.ItemSprite.Sprite) => Item = item;
    
    public GroundItem(Item item, MapInstance mapInstance, Point point)
        : base(item.DisplayName, mapInstance, point, item.Template.ItemSprite.Sprite) => Item = item;

    public override void OnClicked(User source)
    {
        //nothing
        //there's a different packet for picking up items
    }
}
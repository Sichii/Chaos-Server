using Chaos.Containers;
using Chaos.Geometry.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Objects.World;

public class GroundItem : NamedEntity
{
    public Item Item { get; }
    
    
    public override void OnClicked(Aisling source)
    {
        //nothing
        //there's a different packet for picking up items
    }
    
    public GroundItem(Item item, MapInstance mapInstance, IPoint point)
        : base(
            item.DisplayName,
            item.Template.ItemSprite.OffsetPanelSprite,
            mapInstance,
            point) =>
        Item = item;
}
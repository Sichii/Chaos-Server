using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;

namespace Chaos.Models.World;

public sealed class GroundItem(Item item, MapInstance mapInstance, IPoint point)
    : GroundEntity(
          item.DisplayName,
          item.ItemSprite.PanelSprite,
          mapInstance,
          point),
      IDialogSourceEntity
{
    public Item Item { get; set; } = item;

    /// <inheritdoc />
    DisplayColor IDialogSourceEntity.Color => Item.Color;

    /// <inheritdoc />
    EntityType IDialogSourceEntity.EntityType => EntityType.Item;

    /// <inheritdoc />
    void IDialogSourceEntity.Activate(Aisling source) => Item.Script.OnUse(source);

    /// <inheritdoc />
    public override void Animate(Animation animation, uint? sourceId = null)
    {
        var targetedAnimation = animation.GetTargetedAnimation(Id, sourceId);

        foreach (var obj in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                       .ThatCanObserve(this))
            obj.Client.SendAnimation(targetedAnimation);
    }

    public override void OnClicked(Aisling source)
    {
        //nothing
        //there's a different packet for picking up items
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        Item.Update(delta);
        base.Update(delta);
    }
}
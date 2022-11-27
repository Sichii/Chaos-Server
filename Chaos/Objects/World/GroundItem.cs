using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

public sealed class GroundItem : GroundEntity
{
    public Item Item { get; set; }

    public GroundItem(Item item, MapInstance mapInstance, IPoint point)
        : base(
            item.DisplayName,
            item.Template.ItemSprite.OffsetPanelSprite,
            mapInstance,
            point) =>
        Item = item;

    /// <inheritdoc />
    public override void Animate(Animation animation, uint? sourceId = null)
    {
        var targetedAnimation = animation.GetTargetedAnimation(Id, sourceId);

        foreach (var obj in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                       .ThatCanSee(this))
            obj.Client.SendAnimation(targetedAnimation);
    }

    public override void OnClicked(Aisling source)
    {
        //nothing
        //there's a different packet for picking up items
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"{{ UId: {Item.UniqueId}, Name: \"{Name}\", Count: {Item.Count}, Loc: \"{ILocation.ToString(this)}\" }}";

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        Item.Update(delta);
        base.Update(delta);
    }
}
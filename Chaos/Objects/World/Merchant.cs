using Chaos.Containers;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

/// <summary>
///     Represents an in-game merchant.
/// </summary>
internal class Merchant : Creature
{
    public override bool IsAlive => true;
    public override StatSheet StatSheet { get; }

    public override CreatureType Type => CreatureType.Merchant;

    public Merchant(
        string name,
        MapInstance mapInstance,
        Point point,
        ushort sprite
    )
        : base(
            name,
            mapInstance,
            point,
            sprite) => StatSheet = StatSheet.Maxed;

    public override void GoldDroppedOn(int amount, User source)
    {
        //TODO: quests or something?
    }

    public override void ItemDroppedOn(byte slot, byte count, User source)
    {
        //TODO: quests or something?
    }

    public override void OnClicked(User source)
    {
        //TODO: open a menu or something
    }

    public override void Update(TimeSpan delta) => base.Update(delta);
}
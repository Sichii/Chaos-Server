using System;
using System.Threading.Tasks;
using Chaos.Containers;
using Chaos.Core.Definitions;
using Chaos.Core.Geometry;
using Chaos.DataObjects;
using Chaos.WorldObjects.Abstractions;

namespace Chaos.WorldObjects;

/// <summary>
///     Represents an in-game merchant.
/// </summary>
internal class Merchant : Creature
{
    public override bool IsAlive => true;
    public override StatSheet StatSheet { get; }

    public override CreatureType Type => CreatureType.Merchant;

    public Merchant(string name, MapInstance mapInstance, Point point, ushort sprite)
        : base(name, mapInstance, point, sprite) => StatSheet = StatSheet.Maxed;

    public override void OnClicked(User source)
    {
        //TODO: open a menu or something
    }
    public override ValueTask OnUpdated(TimeSpan delta) => throw new NotImplementedException();

    public override void GoldDroppedOn(int amount, User source)
    {
        //TODO: quests or something?
        return;
    }

    public override void ItemDroppedOn(byte slot, int count, User source)
    {
        //TODO: quests or something?
        return;
    }
}
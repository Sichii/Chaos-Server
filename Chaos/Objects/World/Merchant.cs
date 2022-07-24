using Chaos.Containers;
using Chaos.Data;
using Chaos.Geometry.Interfaces;
using Chaos.Objects.World.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Objects.World;

public class Merchant : Creature
{
    public override bool IsAlive => true;
    public override StatSheet StatSheet => StatSheet.Maxed;

    public override CreatureType Type => CreatureType.Merchant;
    
    /*
    public override void OnGoldDroppedOn(int amount, User source)
    {
        //TODO: quests or something?
    }

    public override void OnItemDroppedOn(byte slot, byte count, User source)
    {
        //TODO: quests or something?
    }

    public override void OnClicked(User source)
    {
        //TODO: open a menu or something
    }
    */

    public Merchant(
        string name,
        ushort sprite,
        MapInstance mapInstance,
        IPoint point
    )
        : base(
            name,
            sprite,
            mapInstance,
            point) { }
}
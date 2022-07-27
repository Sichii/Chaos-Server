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
    protected override ILogger<Merchant> Logger { get; }

    public override void OnGoldDroppedOn(int amount, Aisling source)
    {
        //TODO: quests or something?
    }

    public override void OnItemDroppedOn(byte slot, byte count, Aisling source)
    {
        //TODO: quests or something?
    }

    public override void OnClicked(Aisling source)
    {
        //TODO: open a menu or something
    }


    public Merchant(
        string name,
        ushort sprite,
        MapInstance mapInstance,
        IPoint point,
        ILogger<Merchant> logger
    )
        : base(
            name,
            sprite,
            mapInstance,
            point) => Logger = logger;
}
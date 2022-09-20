using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Objects.World;

public sealed class Merchant : Creature
{
    /// <inheritdoc />
    public override int AssailIntervalMs => 500;
    public override bool IsAlive => true;
    public override StatSheet StatSheet => StatSheet.Maxed;

    public override CreatureType Type => CreatureType.Merchant;
    protected override ILogger<Merchant> Logger { get; }

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

    public override void ApplyDamage(
        Creature source,
        int amount,
        byte? hitSound = 1
    ) { }

    public override void OnClicked(Aisling source)
    {
        //TODO: open a menu or something
    }

    public override void OnGoldDroppedOn(Aisling source, int amount)
    {
        //TODO: quests or something?
    }

    public override void OnItemDroppedOn(Aisling source, byte slot, byte count)
    {
        //TODO: quests or something?
    }
}
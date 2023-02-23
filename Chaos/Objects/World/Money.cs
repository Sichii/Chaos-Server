using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Networking.Definitions;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

public sealed class Money : GroundEntity
{
    public int Amount { get; }

    public Money(int amount, MapInstance mapInstance, IPoint point)
        : base(
            "Coins",
            GetSprite(amount),
            mapInstance,
            point) => Amount = amount;

    public static ushort GetSprite(int amount)
    {
        var sprite = amount switch
        {
            >= 5000 => 140,
            >= 1000 => 141,
            >= 500  => 142,
            >= 100  => 137,
            >= 1    => 138,
            _       => 139
        };

        return (ushort)(sprite + NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET);
    }

    public override void OnClicked(Aisling source) { }
}
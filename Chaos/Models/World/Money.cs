using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World.Abstractions;

namespace Chaos.Models.World;

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

        return (ushort)sprite;
    }

    public override void OnClicked(Aisling source)
    {
        //nothing
        //there's a different packet for picking up money
    }
}
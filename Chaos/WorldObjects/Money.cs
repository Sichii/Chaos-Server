using Chaos.Containers;
using Chaos.Core.Definitions;
using Chaos.Core.Geometry;
using Chaos.WorldObjects.Abstractions;

namespace Chaos.WorldObjects;

public class Money : VisibleObject
{
    public int Amount { get; }

    public Money(int amount, MapInstance mapInstance, Point point)
        : base(mapInstance, point, GetSprite(amount)) => Amount = amount;

    public Money(int amount)
        : base(default!, default, GetSprite(amount)) { }

    public static byte GetSprite(int amount) => amount switch
    {
        >= 5000 => 140,
        >= 1000 => 141,
        >= 500  => 142,
        >= 100  => 137,
        >= 1    => 138,
        _       => 139
    };

    public override void OnClicked(User source)
    {
        MapInstance.RemoveObject(this);
        source.Gold += Amount;
        source.Client.SendAttributes(StatUpdateType.ExpGold);
    }
}
using Chaos.Containers;
using Chaos.Geometry.Interfaces;
using Chaos.Objects.World.Abstractions;
using Chaos.Services.Hosted.Options;

namespace Chaos.Objects.World;

public class Money : VisibleEntity
{
    public int Amount { get; init; }
    
    public static byte GetSprite(int amount) => amount switch
    {
        >= 5000 => 140,
        >= 1000 => 141,
        >= 500  => 142,
        >= 100  => 137,
        >= 1    => 138,
        _       => 139
    };

    public Money(int amount, MapInstance mapInstance, IPoint point)
        : base(GetSprite(amount), mapInstance, point) => Amount = amount;
    
    
    public override void OnClicked(Aisling source)
    {
        if (source.Gold + Amount > WorldOptions.Instance.MaxGoldHeld)
        {
            source.Client.SendServerMessage(
                ServerMessageType.ActiveMessage,
                $"You can't hold more than {WorldOptions.Instance.MaxGoldHeld} gold");
            
            return;
        }
        
        MapInstance.RemoveObject(this);
        source.Gold += Amount;
        source.Client.SendAttributes(StatUpdateType.ExpGold);
    }
}
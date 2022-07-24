using Chaos.Containers;
using Chaos.Data;
using Chaos.Geometry.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

public class Monster : Creature
{
    public List<Item> Items { get; }
    public override StatSheet StatSheet { get; }
    public sealed override CreatureType Type { get; }
    
    
    /*
    public override void OnGoldDroppedOn(int amount, User source)
    {
        if ((uint)Gold + amount > int.MaxValue)
            return;

        source.Gold -= amount;
        Gold += amount;

        source.Client.SendAttributes(StatUpdateType.ExpGold);
    }

    public override void OnItemDroppedOn(byte slot, byte count, User source)
    {
        if (source.Inventory.RemoveQuantity(slot, count, out var item))
        {
            Logger.LogDebug(
                "{UserName} dropped {Item} on monster {MonsterName}",
                source.Name,
                item,
                Name);
            Items.Add(item);
        }
    }

    public override void OnClicked(User source)
    {
        var now = DateTime.UtcNow;

        if (LastClicked.TryGetValue(source.Id, out var lastClicked))
            if (now.Subtract(lastClicked).TotalMilliseconds < 1000)
                return;

        LastClicked[source.Id] = now;
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, Name);
    }
    */
    
    public Monster(
        string name,
        ushort sprite,
        MapInstance mapInstance,
        IPoint point,
        CreatureType type = CreatureType.Normal
    )
        : base(
            name,
            sprite,
            mapInstance,
            point)
    {
        Items = new List<Item>();
        Type = type;
        StatSheet = new StatSheet();
    }
}
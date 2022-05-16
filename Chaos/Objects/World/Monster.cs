using Chaos.Containers;
using Chaos.Core.Collections.Synchronized;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

/// <summary>
///     Represents an in-game enemy, or monster.
/// </summary>
internal class Monster : Creature
{
    public SynchronizedList<Item> Items { get; }
    public override CreatureType Type => CreatureType.Normal;

    public Monster(
        string name,
        MapInstance mapInstance,
        Point point,
        ushort sprite
    )
        : base(
            name,
            mapInstance,
            point,
            sprite) => Items = new SynchronizedList<Item>();

    public override void GoldDroppedOn(int amount, User source)
    {
        if ((uint)Gold + amount > int.MaxValue)
            return;

        source.Gold -= amount;
        Gold += amount;

        source.Client.SendAttributes(StatUpdateType.ExpGold);
    }

    public override void ItemDroppedOn(byte slot, byte count, User source)
    {
        if (source.Inventory.RemoveQuantity(slot, count, out var item))
            Items.Add(item);
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

    public override void Update(TimeSpan delta) => base.Update(delta);
}
using Newtonsoft.Json;
using System;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class Item : PanelObject
    {
        private TimeSpan DefaultCooldown => new TimeSpan(0, 0, 0, 0, 250);
        [JsonProperty]
        internal byte Color { get; }
        [JsonProperty]
        internal bool Stackable { get; }
        [JsonProperty]
        internal uint MaxDurability { get; }
        [JsonProperty]
        internal byte Weight { get; }
        [JsonProperty]
        internal int Count;
        [JsonProperty]
        internal uint CurrentDurability;
        [JsonProperty]
        internal bool AccountBound { get; }
        [JsonProperty]
        internal EquipmentSlot EquipmentSlot { get; }

        internal Item(byte slot, ushort sprite, string name, int count, TimeSpan cooldown,
            EquipmentSlot equipmentSlot = EquipmentSlot.None, bool accountBound = false, byte color = 0, bool stackable = false, uint maximumDurability = 0, uint currentDurability = 0, byte weight = 1)
            :base(slot, (ushort)(sprite + Game.ITEM_SPRITE_OFFSET), name, cooldown)
        {
            EquipmentSlot = EquipmentSlot.None;
            Color = color;
            Count = count;
            Stackable = stackable;
            MaxDurability = maximumDurability;
            CurrentDurability = currentDurability;
            Weight = weight;
            AccountBound = accountBound;
        }

        [JsonConstructor]
        internal Item(byte slot, ushort sprite, string name, TimeSpan cooldown, EquipmentSlot equipmentSlot, bool accountBound, byte color, int count, bool stackable, uint maxDurability, uint currentDurability, byte weight)
            :base(slot, sprite, name, cooldown)
        {
            EquipmentSlot = equipmentSlot;
            Color = color;
            Count = count;
            Stackable = stackable;
            MaxDurability = maxDurability;
            CurrentDurability = currentDurability;
            Weight = weight;
            AccountBound = accountBound;
        }

        internal GroundItem GroundItem(Point point, Map map, int count) => new GroundItem(Sprite, point, map,
            new Item(0, Sprite, Name, Cooldown, EquipmentSlot, AccountBound, Color, count, Stackable, MaxDurability, CurrentDurability, Weight));
    }
}

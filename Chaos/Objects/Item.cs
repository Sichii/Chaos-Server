using Newtonsoft.Json;
using System;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class Item : PanelObject
    {
        [JsonProperty]
        internal EquipmentSlot EquipmentSlot { get; }
        [JsonProperty]
        internal byte Color { get; }
        [JsonProperty]
        internal int Count { get; set; }
        [JsonProperty]
        internal bool Stackable { get; }
        [JsonProperty]
        internal uint MaxDurability { get; }
        [JsonProperty]
        internal uint CurrentDurability { get; set; }

        internal Item(byte slot, ushort sprite, byte color, string name, int count, bool stackable, uint maximumDurability, uint currentDurability, TimeSpan cooldown)
            :base(slot, sprite, name, cooldown)
        {
            EquipmentSlot = EquipmentSlot.None;
            Color = color;
            Count = count;
            Stackable = stackable;
            MaxDurability = maximumDurability;
            CurrentDurability = currentDurability;
        }

        [JsonConstructor]
        internal Item(byte slot, ushort sprite, string name, TimeSpan cooldown, EquipmentSlot equipmentSlot, byte color, int count, bool stackable, uint maxDurability, uint currentDurability)
            :base(slot, sprite, name, cooldown)
        {
            EquipmentSlot = equipmentSlot;
            Color = color;
            Count = count;
            Stackable = stackable;
            MaxDurability = maxDurability;
            CurrentDurability = currentDurability;
        }

        internal GroundItem GroundItem(Point point, Map map, int count) => new GroundItem(Sprite, point, map,
            new Item(0, Sprite, Name, Cooldown, EquipmentSlot, Color, count, Stackable, MaxDurability, CurrentDurability));
    }
}

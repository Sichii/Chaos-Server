using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

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
        internal Tuple<EquipmentSlot, ushort> EquipmentPair { get; }
        internal Tuple<ushort, ushort> SpritePair => new Tuple<ushort, ushort>(base.Sprite, (ushort)(base.Sprite + CONSTANTS.ITEM_SPRITE_OFFSET));

        internal Item(byte slot, ushort sprite, string name, int count, TimeSpan cooldown,
            Tuple<EquipmentSlot, ushort> equipmentPair = null, bool accountBound = false, byte color = 0, bool stackable = false, uint maximumDurability = 0, uint currentDurability = 0, byte weight = 1)
            :base(slot, sprite, name, cooldown)
        {
            EquipmentPair = equipmentPair;
            Color = color;
            Count = count;
            Stackable = stackable;
            MaxDurability = maximumDurability;
            CurrentDurability = currentDurability;
            Weight = weight;
            AccountBound = accountBound;
        }

        [JsonConstructor]
        internal Item(byte slot, ushort sprite, string name, TimeSpan cooldown, Tuple<EquipmentSlot, ushort> equipmentPair, bool accountBound, byte color, int count, bool stackable, uint maxDurability, uint currentDurability, byte weight)
            :base(slot, sprite, name, cooldown)
        {
            EquipmentPair = equipmentPair;
            Color = color;
            Count = count;
            Stackable = stackable;
            MaxDurability = maxDurability;
            CurrentDurability = currentDurability;
            Weight = weight;
            AccountBound = accountBound;
        }

        internal GroundItem GroundItem(Point point, Map map, int count) => new GroundItem(SpritePair.Item2, point, map,
            new Item(0, SpritePair.Item1, Name, Cooldown, EquipmentPair, AccountBound, Color, count, Stackable, MaxDurability, CurrentDurability, Weight));
    }
}

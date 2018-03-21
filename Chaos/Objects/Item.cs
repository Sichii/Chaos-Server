// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        internal EquipmentSlot EquipmentSlot;
        [JsonProperty]
        internal ItemSprite ItemSprite;
        [JsonProperty]
        internal Gender Gender;
        internal ushort DisplaySprite => ItemSprite.DisplaySprite;
        internal override bool CanUse => DateTime.UtcNow.Subtract(LastUse).TotalMilliseconds >= CONSTANTS.GLOBAL_ITEM_COOLDOWN_MS;

        /// <summary>
        /// Constructor for basic unusable item.
        /// </summary>
        internal Item(ItemSprite itemSprite, byte color, string name, bool stackable, int count, byte weight, bool accountBound)
            : this(0, itemSprite, color, name, TimeSpan.Zero, EquipmentSlot.None, stackable, count, 0, 0, weight, Gender.Unisex, Animation.None, BodyAnimation.None, accountBound) { }

        /// <summary>
        /// Constructor for equipment.
        /// </summary>
        internal Item(ItemSprite itemSprite, byte color, string name, EquipmentSlot equipmentSlot, uint maxDurability, uint currentDurability, byte weight, Gender gender, bool accountBound)
            : this(0, itemSprite, color, name, TimeSpan.Zero, equipmentSlot, false, 1, maxDurability, currentDurability, weight, gender, Animation.None, BodyAnimation.None, accountBound) { }

        /// <summary>
        /// Constructor for usable item.
        /// </summary>
        internal Item(ItemSprite itemSprite, byte color, string name, TimeSpan cooldown, byte weight, Animation effectAnimation, BodyAnimation bodyAnimation, bool accountBound)
            : this(0, itemSprite, color, name, cooldown, EquipmentSlot.None, false, 1, 0, 0, weight, Gender.Unisex, effectAnimation, bodyAnimation, accountBound) { }

        /// <summary>
        /// Master constructor.
        /// </summary>
        [JsonConstructor]
        internal Item(byte slot, ItemSprite itemSprite, byte color, string name, TimeSpan cooldown, EquipmentSlot equipmentSlot,
            bool stackable, int count, uint maxDurability, uint currentDurability, byte weight, Gender gender,
            Animation effectAnimation, BodyAnimation bodyAnimation, bool accountBound)
            :base(slot, itemSprite.InventorySprite, name, cooldown, effectAnimation, bodyAnimation)
        {
            ItemSprite = itemSprite;
            EquipmentSlot = equipmentSlot;
            Color = color;
            Count = count;
            Stackable = stackable;
            MaxDurability = maxDurability;
            CurrentDurability = currentDurability;
            Weight = weight;
            Gender = gender;
            AccountBound = accountBound;
        }

        /// <summary>
        /// Creates a groundItem from the item object, with the item stored inside of it.
        /// </summary>
        /// <param name="point">Map point of the ground item to create.</param>
        /// <param name="map">Map object the ground item will be on.</param>
        /// <param name="count">Number of the item you'd like placed on the ground.</param>
        internal GroundItem GroundItem(Point point, Map map, int count) => new GroundItem(ItemSprite.OffsetSprite, point, map,
            new Item(0, ItemSprite, Color, Name, Cooldown, EquipmentSlot, Stackable, count, MaxDurability, CurrentDurability, Weight, Gender, EffectAnimation, BodyAnimation, AccountBound));

        /// <summary>
        /// Split a stackable item, update the count for the old item and return a new item object.
        /// </summary>
        /// <param name="count">Number you want to remove from the old stack and return.</param>
        internal Item Split(int count)
        {
            if (Stackable && Count > count)
            {
                Count -= count;
                return new Item(0, ItemSprite, Color, Name, Cooldown, EquipmentSlot, Stackable, count, MaxDurability, CurrentDurability, Weight, Gender, EffectAnimation, BodyAnimation, AccountBound);
            }

            return null;
        }
    }
}

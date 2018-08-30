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

namespace Chaos
{
    /// <summary>
    /// Represents an object that exists within the inventory panel.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class Item : PanelObject
    {
        internal byte Color { get; }
        internal bool Stackable { get; }
        internal uint MaxDurability { get; }
        internal byte Weight { get; }
        [JsonProperty]
        internal int Count;
        [JsonProperty]
        internal uint CurrentDurability;
        internal bool AccountBound { get; }
        internal EquipmentSlot EquipmentSlot { get; }
        internal ItemSprite ItemSprite { get; }
        internal Gender Gender { get; }
        internal ushort NextDialogId { get; }

        /// <summary>
        /// Whether or not the item is usable, based on the global cooldown, and the item's cooldown.
        /// </summary>
        internal override bool CanUse => Elapsed.TotalMilliseconds >= CONSTANTS.GLOBAL_ITEM_COOLDOWN_MS && base.CanUse;

        /// <summary>
        /// Constructor for basic unusable item. These items have only a default activation that will add them to a trade if used during an exchange.
        /// </summary>
        internal Item(ItemSprite itemSprite, byte color, string name, bool stackable = false, int count = 1, byte weight = 1, bool accountBound = false)
            : this(0, itemSprite, color, name, TimeSpan.Zero, EquipmentSlot.None, stackable, count, 0, 0, weight, Gender.Unisex, 0, Animation.None, TargetsType.None, false, BodyAnimation.None, 
                  0, Effect.None, accountBound) { }

        /// <summary>
        /// Constructor for equipment. These items are equippable when used, and also have the default activation.
        /// </summary>
        internal Item(ItemSprite itemSprite, byte color, string name, EquipmentSlot equipmentSlot, uint maxDurability, uint currentDurability, byte weight, Gender gender, bool accountBound = false)
            : this(0, itemSprite, color, name, TimeSpan.Zero, equipmentSlot, false, 1, maxDurability, currentDurability, weight, gender, 0, Animation.None, TargetsType.None, false, BodyAnimation.None, 
                  0, Effect.None, accountBound) { }

        /// <summary>
        /// Constructor for usable item. These items are usable and generally have some kind of effect. NOT EQUIPMENT.
        /// </summary>
        internal Item(ItemSprite itemSprite, byte color, string name, TimeSpan baseCooldown, byte weight, ushort nextDialogId, Animation effectAnimation = default(Animation), 
            TargetsType targetType = TargetsType.None, bool usersOnly = false, BodyAnimation bodyAnimation = BodyAnimation.None, int baseDamage = 0, Effect effect = default(Effect), bool accountBound = false)
            : this(0, itemSprite, color, name, baseCooldown, EquipmentSlot.None, false, 1, 0, 0, weight, Gender.Unisex, nextDialogId, effectAnimation, targetType, usersOnly, bodyAnimation, 
                  baseDamage, effect, accountBound) { }

        /// <summary>
        /// Master constructor for an object that exists within the inventory or equipment panel.
        /// </summary>
        private Item(byte slot, ItemSprite itemSprite, byte color, string name, TimeSpan baseCooldown, EquipmentSlot equipmentSlot, bool stackable, int count, uint maxDurability, uint currentDurability, 
            byte weight, Gender gender, ushort nextDialogId, Animation effectAnimation, TargetsType targetType, bool usersOnly, BodyAnimation bodyAnimation, int baseDamage, Effect effect, 
            bool accountBound)
            :base(slot, itemSprite.InventorySprite, name, baseCooldown, effectAnimation, targetType, usersOnly, bodyAnimation, baseDamage, effect)
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
            NextDialogId = nextDialogId;
            AccountBound = accountBound;
        }

        /// <summary>
        /// Json constructor for an item. Minimal information is serialized, as we retreive the item from the creation engine, and apply persistent information to it.
        /// </summary>
        [JsonConstructor]
        private Item(byte slot, string name, TimeSpan elapsed, int count, uint currentDurability)
            :base(slot, name, elapsed)
        {
            Count = count;
            CurrentDurability = currentDurability;
        }

        /// <summary>
        /// Creates a groundItem from the item object, with the item stored inside of it.
        /// </summary>
        /// <param name="point">Map point of the ground item to create.</param>
        /// <param name="map">Map object the ground item will be on.</param>
        /// <param name="count">Number of the item you'd like placed on the ground.</param>
        internal GroundObject GroundItem(Point point, Map map, int count) => new GroundObject(ItemSprite.OffsetSprite, point, map,
            new Item(0, ItemSprite, Color, Name, BaseCooldown, EquipmentSlot, Stackable, count, MaxDurability, CurrentDurability, Weight, Gender, NextDialogId, Animation, TargetType, 
                UsersOnly, BodyAnimation, BaseDamage, Effect, AccountBound));

        /// <summary>
        /// Split a stackable item, update the count for the old item and return a new item object.
        /// </summary>
        /// <param name="count">Number you want to remove from the old stack and return.</param>
        internal Item Split(int count)
        {
            if (Stackable && Count > count)
            {
                Count -= count;
                return new Item(0, ItemSprite, Color, Name, BaseCooldown, EquipmentSlot, Stackable, count, MaxDurability, CurrentDurability, Weight, Gender, NextDialogId, Animation, 
                    TargetType, UsersOnly, BodyAnimation, BaseDamage, Effect, AccountBound);
            }

            return null;
        }
    }
}

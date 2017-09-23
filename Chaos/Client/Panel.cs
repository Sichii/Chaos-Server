using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class Panel<T> : IEnumerable<T> where T : PanelObject
    {
        [JsonProperty]
        private byte length;
        [JsonProperty]
        private Dictionary<byte, T> Objects { get; set; }
        [JsonProperty]
        private byte[] Invalid { get; }
        internal T this[EquipmentSlot slot] => this[(byte)slot];
        internal T this[string name] => Objects.Values.FirstOrDefault(obj => obj.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        public IEnumerator<T> GetEnumerator() => Objects.Values.ToList().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal T this[byte slot]
        {
            get { return Valid(slot) ? Objects[slot] : null; }
            private set { if (value.Slot == slot && Valid(slot)) Objects[slot] = value; }
        }
        internal byte Length => (byte)(length - 1);

        internal Panel(byte length)
        {
            this.length = length;
            Objects = new Dictionary<byte, T>();
            for (byte i = 0; i < length; i++)
                Objects[i] = null;

            if (length == 90) //skillbook and spellbook
                Invalid = new byte[] { 36, 72 };
            else if (length == 61) //inventory
                Invalid = new byte[] { };
            else if (length == 20) //equipment
                Invalid = new byte[] { };
        }

        [JsonConstructor]
        internal Panel(byte length, Dictionary<byte, T> objects, byte[] invalid)
        {
            this.length = length;
            Objects = objects;
            Invalid = invalid;
        }

        /// <summary>
        /// Custom contains method
        /// </summary>
        /// <param name="obj">Object to check if the panel contains</param>
        internal bool Contains(T obj) => Objects.Values.Contains(obj);

        /// <summary>
        /// Makes sure the slot is valid.
        /// </summary>
        /// <param name="slot">Slot to check.</param>
        private bool Valid(byte slot) => slot > 0 && !Invalid.Contains(slot) && slot < Length;

        /// <summary>
        /// Checks the collection for a stackable item that matches the given object. Increments the count, and rereferences to the given object.
        /// </summary>
        /// <param name="obj">Object to check.</param>
        /// <returns></returns>
        internal bool TryAddStack(T obj)
        {
            if (obj is Item)
            {
                Item objItem = obj as Item;
                Item existingItem = Objects.Values.FirstOrDefault(item => item != null && item.Sprite == objItem.SpritePair.Item1 && item.Name.Equals(objItem.Name) && (item as Item)?.Stackable == true) as Item;
                if (objItem.Stackable && existingItem?.Stackable == true)
                {
                    objItem.Count += existingItem.Count;
                    objItem.Slot = existingItem.Slot;
                    Objects[existingItem.Slot] = obj;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to equip the item at the given slot, return the item that was already occupying the spot if there was one
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool TryEquip(T item, out T outItem)
        {
            outItem = null;
            EquipmentSlot slot = (item as Item).EquipmentPair.Item1;

            if (slot == EquipmentSlot.None || !Valid((byte)slot))
            {
                outItem = null;
                return false;
            }

            if (Objects[(byte)slot] != null && !TryUnequip(slot, out outItem))
                return false;

            if (Objects[(byte)slot] == null)
                Objects[(byte)slot] = item;

            return Objects[(byte)slot] == item;
        }

        /// <summary>
        /// Attempts to unequip the item at EquipmentSlot and place it in the next inventory slot
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        internal bool TryUnequip(EquipmentSlot slot, out T item)
        {
            return TryGetRemove((byte)slot, out item);
        }

        /// <summary>
        /// Attempts to add an object to the object's slot.
        /// </summary>
        /// <param name="obj">Object to add.</param>
        internal bool TryAdd(T obj)
        {
            if (TryAddStack(obj))
                return Objects[obj.Slot] == obj;

            if (Objects[obj.Slot] == null)
                this[obj.Slot] = obj;

            return Objects[obj.Slot] == obj;
        }

        /// <summary>
        /// Attempts to add an object to the next existing slot, adding to existing stackables first.
        /// </summary>
        /// <param name="obj">Object to add</param>
        /// <returns></returns>
        internal bool AddToNextSlot(T obj)
        {
            if (TryAddStack(obj))
                return Objects[obj.Slot] == obj;

            foreach (byte key in Objects.Keys)
            {
                if (!Valid(key))
                    continue;

                obj.Slot = key;
                if (TryAdd(obj))
                    return true;
            }

            return Objects[obj.Slot] == obj;
        }

        /// <summary>
        /// Sets the value of the slot to null. Returns false if invalid slot.
        /// </summary>
        /// <param name="slot">Slot to remove.</param>
        internal bool TryRemove(byte slot)
        {
            if (Valid(slot))
                Objects[slot] = null;

            return Objects[slot] == null;
        }

        /// <summary>
        /// Attempts <see cref="TryRemove(byte)"/> while returning the value.
        /// </summary>
        /// <param name="slot">Slot to remove.</param>
        /// <param name="obj">Return object if successful.</param>
        internal bool TryGetRemove(byte slot, out T obj)
        {
            if (TryGet(slot, out obj) && TryRemove(slot))
                return true;

            return false;
        }

        /// <summary>
        /// Attempts to retreive the objects at slot location to obj reference.
        /// </summary>
        /// <param name="slot">Slot to retreive from.</param>
        /// <param name="obj">Obj reference to set.</param>
        /// <returns></returns>
        internal bool TryGet(byte slot, out T obj)
        {
            obj = Objects[slot];
            return Valid(slot);
        }

        /// <summary>
        /// Attempts <see cref="TryGetRemove(byte, out T)"/> on each slot, then <see cref="TryAdd(T)"/> to swap places.
        /// If either fails, items will be put back.
        /// </summary>
        /// <param name="slot1">First slot to swap.</param>
        /// <param name="slot2">Second slot to swap.</param>
        internal bool TrySwap(byte slot1, byte slot2)
        {
            T one;
            T two;
            if (TryGetRemove(slot1, out one) && TryGetRemove(slot2, out two))
            {
                if (one != null)
                    one.Slot = slot2;
                if (two != null)
                    two.Slot = slot1;

                if (one != null)
                    Objects[slot2] = one;
                if (two != null)
                    Objects[slot1] = two;
                
                return true;
            } //puts the first object back if it succeeded on the first operation, but failed on the second
            else if (one != null)
                TryAdd(one);

            return false;
        }
    }
}

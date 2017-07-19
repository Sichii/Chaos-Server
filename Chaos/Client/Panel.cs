using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class Panel<T> : IEnumerable<T> where T : Objects.PanelObject
    {
        [JsonProperty]
        internal byte Length { get; set; }
        [JsonProperty]
        internal Dictionary<byte, T> Objects { get; set; }
        [JsonProperty]
        internal byte[] Invalid { get; set; }
        internal T this[string name] => Objects.Values.FirstOrDefault(obj => obj.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        public IEnumerator<T> GetEnumerator() => Objects.Values.ToList().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal T this[byte slot]
        {
            get { return Valid(slot) ? Objects[slot] : null; }
            set { if (value.Slot == slot) TryAdd(value); }
        }

        internal Panel(byte length)
        {
            Length = length;
            Objects = new Dictionary<byte, T>();
            for (byte i = 0; i < length; i++)
                Objects[i] = null;

            if (length == 90) //skillbook and spellbook
                Invalid = new byte[] { 36, 72, 89 };
            else if (length == 61) //inventory
                Invalid = new byte[] { 60 };
            else if (length == 20) //equipment
                Invalid = new byte[] { 19 };
        }

        [JsonConstructor]
        internal Panel(byte length, Dictionary<byte, T> objects, byte[] invalid)
        {
            Length = length;
            Objects = objects;
            Invalid = invalid;
        }


        /// <summary>
        /// Makes sure the slot is valid.
        /// </summary>
        /// <param name="slot">Slot to check.</param>
        private bool Valid(byte slot) => slot > 0 && !Invalid.Contains(slot) && slot < Length;

        /// <summary>
        /// Overwrites or adds an object at the object's slot location.
        /// </summary>
        /// <param name="obj">Object to add.</param>
        internal bool TryAdd(T obj)
        {
            if(obj is Objects.Item)
            {
                Objects.Item item = this[obj.Slot] as Objects.Item;
                if (item != null && item.Stackable)
                {
                    item.Count += (obj as Objects.Item).Count;
                    return true;
                }
            }

            if (Valid(obj.Slot) && Objects[obj.Slot] == null)
            {
                Objects[obj.Slot] = obj;
                return true;
            }
            return false;
        }

        internal bool AddToNextSlot(T obj)
        {
            if (obj is Objects.Item)
            {
                Objects.Item existingItem = Objects.Values.FirstOrDefault(item => item != null && item.Sprite == obj.Sprite && item.Name.Equals(obj.Name)) as Objects.Item;
                if (existingItem != null)
                {
                    existingItem.Count += (obj as Objects.Item).Count;
                    return true;
                }
            }

            foreach (var kvp in Objects)
                if (Valid(kvp.Key) && kvp.Value == null)
                {
                    obj.Slot = kvp.Key;
                    return TryAdd(obj);
                }
            return false;
        }

        /// <summary>
        /// Sets the value of the slot to null. Returns false if invalid slot.
        /// </summary>
        /// <param name="slot">Slot to remove.</param>
        internal bool TryRemove(byte slot)
        {
            if (Valid(slot))
            {
                Objects[slot] = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts <see cref="TryRemove(byte)"/> while returning the value.
        /// </summary>
        /// <param name="slot">Slot to remove.</param>
        /// <param name="obj">Return object if successful.</param>
        internal bool TryGetRemove(byte slot, out T obj)
        {
            obj = Objects[slot];
            if (TryRemove(slot))
                return true;

            obj = null;
            return false;
        }

        /// <summary>
        /// Attempts <see cref="TryGetRemove(byte, out T)"/> on each slot, then <see cref="TryAdd(T)"/> to swap places.
        /// If either fails, items will be put back.
        /// </summary>
        /// <param name="slot1">First slot to swap.</param>
        /// <param name="slot2">Second slot to swap.</param>
        internal bool TrySwap(byte slot1, byte slot2)
        {
            T one, two;
            if (TryGetRemove(slot1, out one) && TryGetRemove(slot2, out two))
            {
                one.Slot = slot2;
                two.Slot = slot1;
                TryAdd(one);
                TryAdd(two);
                
                return true;
            } //puts the first object back if it succeeded on the first operation, but failed on the second
            else if (one != null)
                TryAdd(one);

            return false;
        }
    }
}

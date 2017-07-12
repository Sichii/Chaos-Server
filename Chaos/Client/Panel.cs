using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal sealed class Panel<T> : IEnumerable<T> where T : Objects.PanelObject
    {
        public IEnumerator<T> GetEnumerator() => Objects.Values.ToList().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal SortedDictionary<byte, T> Objects { get; private set; }
        internal byte Length { get; }
        internal T this[string name] => Objects.Values.FirstOrDefault(obj => obj.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        internal T this[byte slot]
        {
            get { return Valid(slot) ? Objects[slot] : null; }
            set { if (value.Slot == slot) Add(value); }
        }
        private byte[] Invalid;

        internal Panel(byte length)
        {
            Length = length;
            Objects = new SortedDictionary<byte, T>();
            for (byte i = 0; i < length; i++)
                Objects[i] = null;

            if (length == 90) //skillbook and spellbook
                Invalid = new byte[] { 36, 72, 89 };
            else if (length == 61) //inventory
                Invalid = new byte[] { 60 };
            else if (length == 20) //equipment
                Invalid = new byte[] { 19 };
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
        internal void Add(T obj)
        {
            if (Valid(obj.Slot))
                Objects[obj.Slot] = obj;
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
        /// Attempts <see cref="TryGetRemove(byte, out T)"/> on each slot, then <see cref="Add(T)"/> to swap places.
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
                Add(one);
                Add(two);
                
                return true;
            } //puts the first object back if it succeeded on the first operation, but failed on the second
            else if (one != null)
                Add(one);

            return false;
        }
    }
}

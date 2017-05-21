using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Insert_Creative_Name
{
    internal sealed class Panel<T> : IEnumerable where T : Objects.PanelObject
    {
        public IEnumerator GetEnumerator() => Objects.GetEnumerator();
        internal SortedDictionary<byte, T> Objects { get; private set; }
        internal byte Length { get; }
        internal T this[string name] => Objects.Values.FirstOrDefault(obj => obj.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        internal T this[byte slot]
        {
            get { return Valid(slot) ? Objects[slot] : default(T); }
            set { if (value.Slot == slot) Add(value); }
        }
        private byte[] Invalid;

        internal Panel(byte length)
        {
            Objects = new SortedDictionary<byte, T>();
            Length = length;

            if(length == 90) //skillbook and spellbook
                Invalid = new byte[] { 36, 72, 89 };
            else //inventory
                Invalid = new byte[] { 60 };
        }

        //validates the slot
        private bool Valid(byte slot) => slot > 0 && !Invalid.Contains(slot) && slot < Length;

        //if the slot is taken, overwrite it, if it's not then create it
        internal void Add(T obj)
        {
            if (Valid(obj.Slot))
                Objects[obj.Slot] = obj;
        }

        //attempts to remove the object at the specified slot. returns false if invalid slot/doesnt exist/fails to remove
        internal bool TryRemove(byte slot) => Valid(slot) && Objects.ContainsKey(slot) && Objects.Remove(slot);

        //attempts to remove the object at the specified slot and return it. returns false if invalid slot/doesnt exist/fails to remove
        internal bool TryGetRemove(byte slot, out T obj)
        {
            if (!Valid(slot) || !Objects.ContainsKey(slot))
                return (obj = null) != null;

            obj = Objects[slot];
            return Objects.Remove(slot);
        }

        //attempts to swap two objects. returns false if it fails to remove either object for any reason.
        internal bool TrySwap(byte slot1, byte slot2)
        {
            if (!Valid(slot1) || !Valid(slot2))
                return false;

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

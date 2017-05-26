using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Insert_Creative_Name
{
    [Serializable]
    internal sealed class Panel<T> : IEnumerable where T : Objects.PanelObject
    {
        public IEnumerator GetEnumerator() => Objects.GetEnumerator();
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

        //validates the slot
        private bool Valid(byte slot) => slot > 0 && !Invalid.Contains(slot) && slot < Length;

        //if the slot is taken, overwrite it, if it's not then create it
        internal void Add(T obj)
        {
            if (Valid(obj.Slot))
                Objects[obj.Slot] = obj;
        }

        //attempts to set the value of the slot to null, otherwise return false
        internal bool TryRemove(byte slot)
        {
            if (Valid(slot))
            {
                Objects[slot] = null;
                return true;
            }
            return false;
        }

        //attempts to remove the object at the specified slot and return it. returns false if invalid slot/doesnt exist/fails to remove
        internal bool TryGetRemove(byte slot, out T obj)
        {
            obj = Objects[slot];
            if (TryRemove(slot))
                return true;

            obj = null;
            return false;
        }

        //attempts to swap two objects. returns false if it fails to remove either object for any reason.
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

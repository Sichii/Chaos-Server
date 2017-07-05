using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal sealed class Legend : IEnumerable<LegendMark>
    {
        public IEnumerator<LegendMark> GetEnumerator() => Marks.Values.ToList().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal byte Length => (byte)Marks.Count;
        internal Dictionary<string, LegendMark> Marks;

        /// <summary>
        /// Represents the object containing the user's legend marks.
        /// </summary>
        internal Legend()
        {
            Marks = new Dictionary<string, LegendMark>();
        }

        /// <summary>
        /// Retreives the legend mark at key location.
        /// </summary>
        /// <param name="key">Key of the legend mark you want returned.</param>
        internal LegendMark this[string key] => Marks.ContainsKey(key) ? Marks[key] : null;

        /// <summary>
        /// Adds or replaces an old legend mark at the mark's key location.
        /// </summary>
        /// <param name="mark">Mark to add or replace.</param>
        internal void Add(LegendMark mark)
        {
            LegendMark mToAdd = this[mark.Key];

            if (mToAdd != null)
            {
                mToAdd.Added = DateTime.UtcNow;
                mToAdd.Count++;
            }
            else
                Marks.Add(mark.Key, mark);
        }
        /// <summary>
        /// Attempts to remove the legend mark at key location.
        /// </summary>
        /// <param name="key">Key of the mark to remove.</param>
        internal bool TryRemove(string key) => Marks.Remove(key);

    }

    
    internal sealed class LegendMark
    {
        private GameTime added;
        internal string Mark { get; set; }
        internal string Key { get; set; }
        internal MarkIcon Icon { get; set; }
        internal MarkColor Color { get; set; }
        internal int Count { get; set; }
        internal DateTime Added
        {
            get { return added.ToDateTime(); }
            set { added = GameTime.FromDateTime(value); }
        }

        /// <summary>
        /// Represents individual mark entries in the Legend object.
        /// </summary>
        /// <param name="key">Key of the mark.</param>
        /// <param name="mark">Text of the mark.</param>
        /// <param name="now">Time the mark was added.</param>
        /// <param name="icon">Icon displayed on the mark.</param>
        /// <param name="color">Text color of the mark.</param>
        internal LegendMark(string key, string mark, DateTime now, MarkIcon icon, MarkColor color)
        {
            Key = key;
            Count = 1;
            Icon = icon;
            Color = color;
            Added = now;
        }

        /// <summary>
        /// Returns string representation of a LegendMark ready for ingame use.
        /// </summary>
        public override string ToString() => Count > 1 ? $@"{Mark} ({Count}) - {added.ToString()}" : $@"{Mark} - {added.ToString()}";
    }
}

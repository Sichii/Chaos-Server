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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class Legend : IEnumerable<LegendMark>
    {
        internal readonly object Sync = new object();
        public IEnumerator<LegendMark> GetEnumerator() => Marks.Select(kvp => kvp.Value).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal byte Length => (byte)Marks.Count;
        [JsonProperty]
        private List<KeyValuePair<string, LegendMark>> Marks { get; }

        /// <summary>
        /// Represents the object containing the user's legend marks.
        /// </summary>
        internal Legend()
        {
            Marks = new List<KeyValuePair<string, LegendMark>>();
        }

        [JsonConstructor]
        internal Legend(List<KeyValuePair<string, LegendMark>> marks)
        {
            Marks = marks;
        }

        /// <summary>
        /// Retreives the legend mark at key location.
        /// </summary>
        /// <param name="key">Key of the legend mark you want returned.</param>
        internal LegendMark this[string key]
        {
            get
            {
                lock (Sync)
                    return Marks.FirstOrDefault(kvp => kvp.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase)).Value;
            }
        }

        /// <summary>
        /// Adds or replaces an old legend mark at the mark's key location.
        /// </summary>
        /// <param name="mark">Mark to add or replace.</param>
        internal void Add(LegendMark mark)
        {
            lock (Sync)
            {
                LegendMark mToAdd = this[mark.Key];

                if (mToAdd != null)
                {
                    mToAdd.Added = DateTime.UtcNow;
                    mToAdd.Count++;
                }
                else
                    Marks.Add(new KeyValuePair<string, LegendMark>(mark.Key, mark));
            }
        }
        /// <summary>
        /// Attempts to remove the legend mark at key location.
        /// </summary>
        /// <param name="key">Key of the mark to remove.</param>
        internal bool TryRemove(string key)
        {
            lock (Sync)
                return Marks.RemoveAll(m => m.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase)) != 0;
        }
    }

    
    internal sealed class LegendMark
    {
        [JsonProperty]
        private GameTime added;
        [JsonProperty]
        internal string Mark { get; set; }
        [JsonProperty]
        internal string Key { get; set; }
        [JsonProperty]
        internal MarkIcon Icon { get; set; }
        [JsonProperty]
        internal MarkColor Color { get; set; }
        [JsonProperty]
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
        internal LegendMark(DateTime now, string mark, string key, MarkIcon icon, MarkColor color)
        {
            Added = now;
            Key = key;
            Mark = mark;
            Count = 1;
            Icon = icon;
            Color = color;
        }

        [JsonConstructor]
        internal LegendMark(GameTime added, string mark, string key, MarkIcon icon, MarkColor color, int count)
        {
            this.added = added;
            Mark = mark;
            Key = key;
            Icon = icon;
            Color = color;
            Count = count;
        }

        /// <summary>
        /// Returns string representation of a LegendMark ready for ingame use.
        /// </summary>
        public override string ToString() => Count > 1 ? $@"{Mark} ({Count}) - {added.ToString()}" : $@"{Mark} - {added.ToString()}";
    }
}

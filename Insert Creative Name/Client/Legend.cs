using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal sealed class Legend : IEnumerable<LegendMark>
    {
        public IEnumerator<LegendMark> GetEnumerator() => Marks.Values.ToList().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal byte Length => (byte)Marks.Count;
        internal Dictionary<string, LegendMark> Marks;

        internal Legend()
        {
            Marks = new Dictionary<string, LegendMark>();
        }

        internal LegendMark this[string key] => Marks.ContainsKey(key) ? Marks[key] : null;

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

        internal bool TryRemove(string key) => Marks.Remove(key);
    }

    internal sealed class LegendMark
    {
        private GameTime added;
        internal string Mark { get; set; }
        internal string Key { get; set; }
        internal byte Icon { get; set; }
        internal byte Color { get; set; }
        internal int Count { get; set; }
        internal DateTime Added
        {
            get { return added.ToDateTime(); }
            set { added = GameTime.FromDateTime(value); }
        }

        internal LegendMark(string key, string mark, DateTime now)
        {
            Key = key;
            Count = 1;
            Added = now;
        }

        public override string ToString()
        {
            return Count > 1 ? $@"{Mark} ({Count}) - {added.ToString()}" : $@"{Mark} - {added.ToString()}";
        }
    }
}

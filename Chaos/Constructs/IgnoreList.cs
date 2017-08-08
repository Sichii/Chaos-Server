using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos
{
    internal class IgnoreList
    {
        private List<string> Names { get; set; }

        internal IgnoreList()
        {
            Names = new List<string>();
        }

        internal bool Contains(string name) => Names.Contains(name, StringComparer.CurrentCultureIgnoreCase);

        internal bool TryAdd(string name)
        {
            if (Contains(name))
                return false;

            Names.Add(name);
            return true;
        }

        internal bool TryRemove(string name)
        {
            if (!Contains(name))
                return false;

            Names.RemoveAll(entry => entry.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            return true;
        }

        public override string ToString() => string.Join(Environment.NewLine, Names.ToArray());
        internal string[] ToArray() => Names.ToArray();
    }
}

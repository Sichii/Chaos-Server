using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos
{
    [Serializable]
    internal sealed class Guild : IEnumerable
    {
        public IEnumerator GetEnumerator() => Members.GetEnumerator();
        internal string Name { get; set; }
        internal List<string> Ranks { get; set; }
        internal Bank Bank { get; set; }
        internal ConcurrentDictionary<string, string> Members { get; set; } //name, rank
        internal string this[string name]
        {
            get { return Members.ContainsKey(name) ? Members[name] : null; }
            private set { Members.AddOrUpdate(name, value, (key, oldValue) => value); }
        }

        internal Guild(string name, List<string> founders)
        {
            Name = name;
        }

        internal string TitleOf(string name) => this[name];

        internal bool TryAddMember(Objects.User user)
        {
            if (user.Guild != null)
                return false;

            return Members.TryAdd(user.Name, Ranks[0]);
        }

        internal bool TryRemoveMember(Objects.User user)
        {
            if (user.Guild == null)
                return false;

            string s = string.Empty;
            return Members.TryRemove(user.Name, out s);
        }
    }
}

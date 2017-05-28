using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal sealed class Group : IEnumerable<Objects.User>
    {
        public IEnumerator<Objects.User> GetEnumerator() => Users.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal Objects.User Leader { get; set; }
        internal List<Objects.User> Users { get; set; }
        internal byte Size => (byte)Users.Count;
        internal Objects.User this[string name] => Users.FirstOrDefault(user => user.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        internal Objects.User this[uint id] => Users.FirstOrDefault(user => user.Id == id);
        internal Objects.GroupBox Box { get; set; }
        internal Group(Objects.User a, Objects.User b)
        {
            Leader = a;
            Users = new List<Objects.User>() { a, b };
        }

        internal Objects.GroupBox CreateBox(Objects.User leader, string groupName, byte maxLevel, byte[] maxAmounts)
        {
            Box = new Objects.GroupBox(leader, groupName, maxLevel, maxAmounts);
            return Box;
        }

        internal bool TryAdd(Objects.User user)
        {
            if (Users.Contains(user))
                return false;

            Users.Add(user);
            return true;
        }

        internal bool TryRemove(Objects.User user)
        {
            if (Users.Contains(user))
                return Users.Remove(user);

            return false;
        }

        internal bool TryRemove(uint id)
        {
            Objects.User user = this[id];

            if (user == null)
                return false;

            return TryRemove(user);
        }

        public override string ToString()
        {
            string groupString = "Group members";

            foreach(Objects.User user in Users)
                if (user == Leader)
                    groupString += $@"\n*  {user.Name}";
                else
                    groupString += $@"\n   {user.Name}";

            groupString += $@"\nTotal {Size}";

            return groupString;
        }
    }
}

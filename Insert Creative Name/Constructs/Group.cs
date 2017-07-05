using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
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

        /// <summary>
        /// Object representing the serverside concept of a Group.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="accepter"></param>
        internal Group(Objects.User sender, Objects.User accepter)
        {
            Leader = sender;
            Users = new List<Objects.User>() { sender, accepter };
        }
        /// <summary>
        /// Creates a GroupBox for this Group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="maxLevel">Max allowed level to join.</param>
        /// <param name="maxAmounts">Class amounts, etc.</param>
        /// <returns></returns>
        internal Objects.GroupBox CreateBox(string groupName, byte maxLevel, byte[] maxAmounts)
        {
            Box = new Objects.GroupBox(Leader, groupName, maxLevel, maxAmounts);
            return Box;
        }
        /// <summary>
        /// Attempts to add a User to the Group.
        /// </summary>
        /// <param name="user">User to be added.</param>
        internal bool TryAdd(Objects.User user)
        {
            if (Users.Contains(user))
                return false;

            Users.Add(user);
            return true;
        }
        /// <summary>
        /// Attempts to remove a User from the Group.
        /// </summary>
        /// <param name="user">User to be removed.</param>
        internal bool TryRemove(Objects.User user)
        {
            if (Users.Contains(user))
                return Users.Remove(user);

            return false;
        }
        /// <summary>
        /// Attempts to remove a User from the Group by using the user's id
        /// </summary>
        /// <param name="id">Id of the user to be removed.</param>
        internal bool TryRemove(uint id)
        {
            Objects.User user = this[id];

            if (user == null)
                return false;

            return TryRemove(user);
        }
        /// <summary>
        /// String representation of the group, ready for ingame use.
        /// </summary>
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

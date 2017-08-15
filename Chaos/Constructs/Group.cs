using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Chaos
{
    internal sealed class Group : IEnumerable<User>
    {
        internal readonly object Sync = new object();
        public IEnumerator<User> GetEnumerator() => Users.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal int GroupId { get; }
        internal User Leader => Users[0];
        private List<User> Users { get; set; }
        internal byte Size => (byte)Users.Count;
        internal GroupBox Box { get; set; }
        internal User this[string name]
        {
            get
            {
                User user;
                TryGet(name, out user);
                return user;
            }
        }
        internal User this[int id]
        {
            get
            {
                User user;
                TryGet(id, out user);
                return user;
            }
        }

        internal Group(User sender, User accepter)
        {
            Users = new List<User>() { sender, accepter };
            GroupId = Interlocked.Increment(ref Server.NextId);
            sender.Group = this;
            accepter.Group = this;
        }

        /// <summary>
        /// Creates a GroupBox for this Group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="maxLevel">Max allowed level to join.</param>
        /// <param name="maxAmounts">Class amounts, etc.</param>
        internal GroupBox CreateBox(string text, byte maxLevel, byte[] maxAmounts)
        {
            lock (Sync)
            {
                Box = new GroupBox(Leader, text, maxLevel, maxAmounts);
                return Box;
            }
        }
        /// <summary>
        /// Attempts to add a User to the Group.
        /// </summary>
        /// <param name="user">User to be added.</param>
        internal bool TryAdd(User user)
        {
            lock (Sync)
            {
                if (Users.Contains(user))
                    return false;

                Users.Add(user);
                user.Group = this;
                return true;
            }
        }
        /// <summary>
        /// Attempts to remove a User from the Group.
        /// </summary>
        /// <param name="user">User to be removed.</param>
        internal bool TryRemove(User user, bool leader = false)
        {
            lock (Sync)
            {
                if (Users.Contains(user))
                    if (Users.Remove(user))
                    {
                        user.Group = null;

                        if (Users.Count == 1)
                        {
                            Users[0].Client.Enqueue(Users[0].Client.Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, "Group has been disbanded."));
                            TryRemove(Users[0]);
                        }
                        else if (Users.Count > 1)
                            foreach (User u in Users)
                                u.Client.Enqueue(u.Client.Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"{user.Name} {(leader ? "has been kicked from the group" : "has left the group")}"));
                        return true;
                    }

                return false;
            }
        }
        /// <summary>
        /// Attempts to remove a User from the Group by using the user's id
        /// </summary>
        /// <param name="id">Id of the user to be removed.</param>
        internal bool TryRemove(int id)
        {
            lock (Sync)
            {
                User user = this[id];

                if (user == null)
                    return false;

                return TryRemove(user);
            }
        }

        /// <summary>
        /// Attempts to retreive the user with the given name, if they exist.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <param name="user">The user reference to set.</param>
        /// <returns></returns>
        internal bool TryGet(string name, out User user)
        {
            lock (Sync)
            {
                user = Users.FirstOrDefault(u => u.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                return user != null;
            }
        }

        /// <summary>
        /// Attempts to retreive the user with the given id, if they exist.
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <param name="user">The user reference to set.</param>
        /// <returns></returns>
        internal bool TryGet(int id, out User user)
        {
            lock (Sync)
            {
                user = Users.FirstOrDefault(u => u.Id == id);
                return user != null;
            }
        }



        /// <summary>
        /// String representation of the group, ready for ingame use.
        /// </summary>
        public override string ToString()
        {
            lock (Sync)
            {
                string groupString = "Group members";

                foreach (User user in Users)
                    if (user == Leader)
                        groupString += $@"\n*  {user.Name}";
                    else
                        groupString += $@"\n   {user.Name}";

                groupString += $@"\nTotal {Size}";

                return groupString;
            }
        }
    }
}

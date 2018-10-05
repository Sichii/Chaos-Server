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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal sealed class Group : IEnumerable<User>
    {
        private readonly object Sync = new object();
        private readonly List<User> Users;

        internal User Leader => Users?[0];
        internal byte Size => (byte)Users.Count;

        internal GroupBox Box { get; private set; }

        public IEnumerator<User> GetEnumerator()
        {
            lock (Sync)
                using (IEnumerator<User> safeEnum = Users.GetEnumerator())
                    while (safeEnum.MoveNext())
                        yield return safeEnum.Current;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Base constructor for an enumerable object of User. Represents a group of users within the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="accepter"></param>
        internal Group(User sender, User accepter)
        {
            Users = new List<User>() { sender, accepter };
            sender.Group = this;
            accepter.Group = this;
        }

        /// <summary>
        /// Synchronously creates a GroupBox for this Group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="maxLevel">Max allowed level to join.</param>
        /// <param name="maxAmounts">Class amounts, etc.</param>
        internal GroupBox CreateBox(string text, byte maxLevel, byte[] maxAmounts)
        {
            lock (Sync)
            {
                return new GroupBox(text, maxLevel, maxAmounts);
            }
        }
        /// <summary>
        /// Attempts to synchronously add a User to the Group.
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
        /// Attempts to synchronously remove a User from the Group.
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
                            Users[0].Client.SendServerMessage(ServerMessageType.ActiveMessage, "Group has been disbanded.");
                            user.Client.SendServerMessage(ServerMessageType.ActiveMessage, "Group has been disbanded.");
                            TryRemove(Users[0]);
                        }
                        else if (Users.Count > 1)
                            foreach (User u in Users)
                                u.Client.SendServerMessage(ServerMessageType.ActiveMessage, $@"{user.Name} {(leader ? "has been kicked from the group" : "has left the group")}");
                        return true;
                    }

                return false;
            }
        }

        /// <summary>
        /// Attempts to synchronously remove a User from the Group by using the user's id
        /// </summary>
        /// <param name="id">Id of the user to be removed.</param>
        internal bool TryRemove(int id)
        {
            lock (Sync)
            {
                return TryGet(id, out User user) && TryRemove(user);
            }
        }

        /// <summary>
        /// Attempts to synchronously retreive the user with the given name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <param name="user">The user reference to set.</param>
        internal bool TryGet(string name, out User user)
        {
            lock (Sync)
            {
                user = Users.FirstOrDefault(u => u.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                return user != null;
            }
        }

        /// <summary>
        /// Attempts to synchronously retreive the user with the given id.
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <param name="user">The user reference to set.</param>
        internal bool TryGet(int id, out User user)
        {
            lock (Sync)
            {
                user = Users.FirstOrDefault(u => u.ID == id);
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
                        groupString += '\n' + $"* {user.Name}";
                    else
                        groupString += '\n' + $"  {user.Name}";

                groupString += '\n' + $"Total {Size}";

                return groupString;
            }
        }
    }
}

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
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class IgnoreList
    {
        private readonly object Sync = new object();
        [JsonProperty]
        private List<string> Names;

        /// <summary>
        /// Default constructor for an object representing a new user's ignore list.
        /// </summary>
        internal IgnoreList()
        {
            Names = new List<string>();
        }

        /// <summary>
        /// Json and Master constructor for an object representing an existing user's ignore list.
        /// </summary>
        /// <param name="names"></param>
        [JsonConstructor]
        internal IgnoreList(List<string> names)
        {
            Names = names;
        }

        /// <summary>
        /// Synchronously and case in-sensitively checks if a name already exists in the list.
        /// </summary>
        /// <param name="name">Name of the person you want to check if it contains.</param>
        internal bool Contains(string name)
        {
            lock (Sync)
                return Names.Contains(name, StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Attempts to synchronously add the specified name to the ignore list.
        /// </summary>
        /// <param name="name">The name to add.</param>
        internal bool TryAdd(string name)
        {
            lock (Sync)
            {
                if (Contains(name))
                    return false;

                Names.Add(name);
                return true;
            }
        }

        /// <summary>
        /// Attempts to synchronously remove the specified name from the ignore list.
        /// </summary>
        /// <param name="name">The name to remove.</param>
        internal bool TryRemove(string name)
        {
            lock (Sync)
            {
                if (!Contains(name))
                    return false;

                Names.RemoveAll(entry => entry.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                return true;
            }
        }

        /// <summary>
        /// Synchronously provides a game-ready string representation of the ignore list
        /// </summary>
        public override string ToString()
        {
            lock (Sync)
                return string.Join(Environment.NewLine, Names.ToArray());
        }
    }
}

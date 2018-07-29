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
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class IgnoreList
    {
        private readonly object Sync = new object();
        [JsonProperty]
        private List<string> Names { get; set; }

        internal IgnoreList()
        {
            Names = new List<string>();
        }

        [JsonConstructor]
        internal IgnoreList(List<string> names)
        {
            Names = names;
        }

        /// <summary>
        /// Custom case in-sensitive contains method
        /// </summary>
        /// <param name="name">Name of the person you want to check if it contains.</param>
        internal bool Contains(string name)
        {
            lock (Sync)
                return Names.Contains(name, StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Attempts to add then specified name to the ignore list.
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
        /// Attempts to remove the specified name from the ignore list.
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
        /// Custom ToString() method, provides a game-ready string representation of the ignore list
        /// </summary>
        public override string ToString()
        {
            lock (Sync)
                return string.Join(Environment.NewLine, Names.ToArray());
        }
    }
}

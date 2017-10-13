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
        /// <returns></returns>
        internal bool Contains(string name) => Names.Contains(name, StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Attempts to add then specified name to the ignore list.
        /// </summary>
        /// <param name="name">The name to add.</param>
        /// <returns></returns>
        internal bool TryAdd(string name)
        {
            if (Contains(name))
                return false;

            Names.Add(name);
            return true;
        }

        /// <summary>
        /// Attempts to remove the specified name from the ignore list.
        /// </summary>
        /// <param name="name">The name to remove.</param>
        /// <returns></returns>
        internal bool TryRemove(string name)
        {
            if (!Contains(name))
                return false;

            Names.RemoveAll(entry => entry.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            return true;
        }

        /// <summary>
        /// Custom ToString() method, provides a game-ready string representation of the ignore list
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Join(Environment.NewLine, Names.ToArray());

        /// <summary>
        /// Custom ToArray() method, to convert the internal list of names to an array of names
        /// </summary>
        /// <returns></returns>
        internal string[] ToArray() => Names.ToArray();
    }
}

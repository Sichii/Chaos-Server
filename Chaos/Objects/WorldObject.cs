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
using System.Threading;

namespace Chaos
{
    /// <summary>
    /// Represents an object that exists within the world.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class WorldObject : IComparable<WorldObject>
    {
        protected internal int Id { get; }
        [JsonProperty]
        protected internal string Name { get; set; }
        protected internal DateTime Creation { get; }

        /// <summary>
        /// Json & Master constructor for an object that exists within the world.
        /// </summary>
        [JsonConstructor]
        protected internal WorldObject(string name)
        {
            Id = Interlocked.Increment(ref Server.NextId);
            Name = name;
            Creation = DateTime.UtcNow;
        }

        public int CompareTo(WorldObject obj) => ReferenceEquals(this, obj) ? 0 : Id.CompareTo(obj.Id);
    }
}

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
using System.Threading;

namespace Chaos
{
    internal abstract class WorldObject : IComparable<WorldObject>
    {
        [JsonProperty]
        protected internal int Id { get; }
        [JsonProperty]
        protected internal string Name { get; set; }
        [JsonProperty]
        protected internal DateTime Creation { get; }

        protected internal WorldObject(string name)
        {
            Id = Interlocked.Increment(ref Server.NextId);
            Name = name;
            Creation = DateTime.UtcNow;
        }

        public int CompareTo(WorldObject obj) => ReferenceEquals(this, obj) ? 0 : Id.CompareTo(obj.Id);
    }

    internal sealed class WorldObjectComparer : IEqualityComparer<WorldObject>
    {
        public bool Equals(WorldObject obj1, WorldObject obj2) => ReferenceEquals(obj1, obj2) ? true : GetHashCode(obj1) == GetHashCode(obj2);
        public int GetHashCode(WorldObject obj) => obj.Id.GetHashCode();
    }
}

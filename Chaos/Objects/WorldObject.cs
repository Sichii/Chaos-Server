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

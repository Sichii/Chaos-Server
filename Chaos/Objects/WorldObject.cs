using Newtonsoft.Json;
using System;
using System.Threading;

namespace Chaos
{
    internal abstract class WorldObject
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
    }
}

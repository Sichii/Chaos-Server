using System;
using System.Threading;

namespace Chaos.Objects
{
    internal abstract class WorldObject
    {
        protected internal int Id { get; }
        protected internal string Name { get; set; }
        protected internal DateTime Creation { get; }

        protected internal WorldObject(string name)
        {
            Id = Interlocked.Increment(ref Server.NextId);
            Name = name;
            Creation = DateTime.UtcNow;
        }
    }
}

using System;

namespace Chaos.Objects
{
    [Serializable]
    internal abstract class WorldObject
    {
        protected internal uint Id { get; }
        protected internal string Name { get; set; }
        protected internal DateTime Creation { get; }

        protected internal WorldObject(uint id, string name)
        {
            Id = id;
            Name = name;
            Creation = DateTime.UtcNow;
        }
    }
}

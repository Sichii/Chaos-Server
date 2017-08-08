using System;

namespace Chaos
{
    internal sealed class Monster : Creature
    {
        internal bool IsActive { get; set; }
        internal TimeSpan UpdateInterval { get; }
        internal DateTime LastUpdate { get; set; }

        internal Monster(string name, ushort sprite, CreatureType type, Point point, Map map, TimeSpan updateInterval, Direction direction = Direction.South)
            : base(name, sprite, type, point, map, direction)
        {
            UpdateInterval = updateInterval;
            LastUpdate = DateTime.MinValue;
            IsActive = false;
        }
    }
}

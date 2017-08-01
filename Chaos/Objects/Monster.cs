using System;

namespace Chaos
{
    internal sealed class Monster : Creature
    {
        internal TimeSpan UpdateInterval { get; }

        internal Monster(string name, ushort sprite, byte type, Point point, Map map, TimeSpan updateInterval, Direction direction = Direction.South)
            : base(name, sprite, type, point, map, direction)
        {
            UpdateInterval = updateInterval;
        }
    }
}

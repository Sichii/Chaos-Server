using System;

namespace Insert_Creative_Name.Objects
{
    internal sealed class Monster : Creature
    {
        internal TimeSpan UpdateInterval { get; }

        internal Monster(uint id, string name, ushort sprite, byte type, Point point, Map map, TimeSpan updateInterval, Direction direction = Direction.South)
            : base(id, name, sprite, type, point, map, direction)
        {
            UpdateInterval = updateInterval;
        }
    }
}

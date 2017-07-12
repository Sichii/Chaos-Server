using System;

namespace Chaos.Objects
{
    internal class Creature : VisibleObject
    {
        internal Direction Direction { get; set; }
        internal byte HealthPercent { get; set; }
        internal byte Type { get; }

        /// <summary>
        /// Object representing an ingame creature.
        /// </summary>
        /// <param name="id">f</param>
        /// <param name="name"></param>
        /// <param name="sprite"></param>
        /// <param name="type"></param>
        /// <param name="point"></param>
        /// <param name="map"></param>
        /// <param name="direction"></param>
        internal Creature(string name, ushort sprite, byte type, Point point, Map map, Direction direction = Direction.South)
            : base(name, sprite, point, map)
        {
            Direction = direction;
            HealthPercent = 100;
            Type = type;
        }
    }
}

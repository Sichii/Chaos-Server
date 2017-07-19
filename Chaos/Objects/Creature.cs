using Newtonsoft.Json;
using System;

namespace Chaos.Objects
{
    internal class Creature : VisibleObject
    {
        [JsonProperty]
        internal Direction Direction { get; set; }
        [JsonProperty]
        internal byte HealthPercent { get; set; }
        [JsonProperty]
        internal byte Type { get; }

        internal Creature(string name, ushort sprite, byte type, Point point, Map map, Direction direction = Direction.South)
            : base(name, sprite, point, map)
        {
            Direction = direction;
            HealthPercent = 100;
            Type = type;
        }
    }
}

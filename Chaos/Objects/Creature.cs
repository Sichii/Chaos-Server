using Newtonsoft.Json;

namespace Chaos
{
    internal class Creature : VisibleObject
    {
        [JsonProperty]
        internal Direction Direction { get; set; }
        [JsonProperty]
        internal byte HealthPercent { get; set; }
        [JsonProperty]
        internal CreatureType Type { get; }

        internal Creature(string name, ushort sprite, CreatureType type, Point point, Map map, Direction direction = Direction.South)
            : base(name, sprite, point, map)
        {
            Direction = direction;
            HealthPercent = 100;
            Type = type;
        }
    }
}

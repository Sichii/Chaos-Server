using Newtonsoft.Json;

namespace Chaos
{
    internal abstract class Creature : VisibleObject
    {
        [JsonProperty]
        internal Direction Direction { get; set; }
        [JsonProperty]
        internal CreatureType Type { get; }
        internal abstract byte HealthPercent { get; }
        internal abstract uint MaximumHP { get; }
        internal abstract uint CurrentHP { get; set; }


        internal Creature(string name, ushort sprite, CreatureType type, Point point, Map map, Direction direction = Direction.South)
            : base(name, sprite, point, map)
        {
            Direction = direction;
            Type = type;
        }
    }
}

namespace Insert_Creative_Name.Objects
{
    internal class Creature : VisibleObject
    {
        internal Direction Direction { get; set; }
        internal byte HealthPercent { get; set; }
        internal byte Type { get; }

        internal Creature(uint id, string name, ushort sprite, byte type, Point point, Map map, Direction direction = Direction.South)
            : base(id, name, sprite, point, map)
        {
            Direction = direction;
            HealthPercent = 100;
            Type = type;
        }
    }
}

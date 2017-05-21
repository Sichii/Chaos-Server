using System.Collections.Generic;

namespace Insert_Creative_Name.Objects
{
    internal sealed class Merchant : Creature
    {
        internal Dictionary<ushort, Dialog> Dialogs;

        internal Merchant(uint id, string name, ushort sprite, byte type, Point point, Map map, Direction direction = Direction.South)
            : base(id, name, sprite, type, point, map, direction)
        {
            Dialogs = new Dictionary<ushort, Dialog>();
        }
    }
}

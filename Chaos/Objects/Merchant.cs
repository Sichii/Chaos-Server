using System.Collections.Generic;

namespace Chaos.Objects
{
    internal sealed class Merchant : Creature
    {
        internal Dictionary<ushort, Dialog> Dialogs;

        internal Merchant(string name, ushort sprite, byte type, Point point, Map map, Direction direction = Direction.South)
            : base(name, sprite, type, point, map, direction)
        {
            Dialogs = new Dictionary<ushort, Dialog>();
        }
    }
}

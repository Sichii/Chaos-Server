using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name.Objects
{
    internal sealed class WorldMapNode
    {
        internal Point ScreenPosition { get; }
        internal string Name { get; }
        internal ushort MapId { get; }
        internal Point TargetPoint { get; }

        internal Location TargetLocation => new Location(MapId, TargetPoint);

        public WorldMapNode(Point position, string name, ushort mapId, Point point)
        {
            ScreenPosition = position;
            Name = name;
            MapId = mapId;
            TargetPoint = point;
        }
    }
}

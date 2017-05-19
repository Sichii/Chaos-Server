using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal struct WorldMapNode
    {
        internal Point Position { get; set; }
        internal string Name { get; set; }
        internal ushort MapId { get; set; }
        internal Point Point { get; set; }

        internal Location TargetLocation
        {
            get
            {
                return new Location(MapId, Point.X, Point.Y);
            }
        }

        public WorldMapNode(Point position, string name, ushort mapId, Point point)
        {
            Position = position;
            Name = name;
            MapId = mapId;
            Point = point;
        }
    }
}

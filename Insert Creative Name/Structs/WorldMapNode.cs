using System.IO;
using System.Text;

namespace Chaos.Objects
{
    internal struct WorldMapNode
    {
        internal Point ScreenPosition { get; }
        internal string Name { get; }
        internal ushort MapId { get; }
        internal Point TargetPoint { get; }
        internal ushort CRC
        {
            get
            {
                using (MemoryStream m = new MemoryStream())
                using (BinaryWriter b = new BinaryWriter(m))
                {
                    b.Write(Encoding.Unicode.GetBytes(Name));
                    b.Write(MapId);
                    b.Write(TargetPoint.X);
                    b.Write(TargetPoint.Y);

                    return CRC16.Calculate(m.ToArray());
                }
            }
        }

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

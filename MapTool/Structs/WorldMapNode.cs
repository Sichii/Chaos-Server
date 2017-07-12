using System.IO;
using System.Text;

namespace MapTool
{
    internal struct WorldMapNode
    {
        internal Point Position { get; }
        internal string Name { get; }
        internal ushort MapId { get; }
        internal Point Point { get; }
        internal Location Location => new Location(MapId, Point);

        public WorldMapNode(Point position, string name, ushort mapId, Point point)
        {
            Position = position;
            Name = name;
            MapId = mapId;
            Point = point;
        }

        internal ushort CRC
        {
            get
            {
                MemoryStream m = new MemoryStream();
                using (BinaryWriter b = new BinaryWriter(m))
                {
                    b.Write(Encoding.Unicode.GetBytes(Name));
                    b.Write(MapId);
                    b.Write(Point.X);
                    b.Write(Point.Y);

                    return CRC16.Calculate(m.ToArray());
                }
            }
        }
    }
}

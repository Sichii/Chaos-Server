using System.IO;
using System.Text;

namespace Chaos
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

        internal ushort CheckSum
        {
            get
            {
                MemoryStream data = new MemoryStream();
                using (BinaryWriter writer = new BinaryWriter(data))
                {
                    writer.Write(Encoding.Unicode.GetBytes(Name));
                    writer.Write(MapId);
                    writer.Write(Point.X);
                    writer.Write(Point.Y);

                    writer.Flush();
                    return Crypto.Generate16(data.ToArray());
                }
            }
        }
    }
}

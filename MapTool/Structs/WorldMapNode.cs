// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

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
                MemoryStream data = new MemoryStream();
                using (BinaryWriter writer = new BinaryWriter(data))
                {
                    writer.Write(Encoding.Unicode.GetBytes(Name));
                    writer.Write(MapId);
                    writer.Write(Point.X);
                    writer.Write(Point.Y);
                    writer.Flush();
                    return CheckSum.Generate16(data.ToArray());
                }
            }
        }
    }
}

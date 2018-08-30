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

namespace Chaos
{
    public struct WorldMapNode
    {
        public Point Position { get; }
        public string Name { get; }
        public ushort MapId { get; }
        public Point Point { get; }
        internal Location Location => new Location(MapId, Point);

        /// <summary>
        /// Master constructor for a structure representing an in-game clickable node on a world map. Position on the world map, and the warp information are stored here.
        /// </summary>
        public WorldMapNode(Point position, string name, ushort mapId, Point point)
        {
            Position = position;
            Name = name;
            MapId = mapId;
            Point = point;
        }

        /// <summary>
        /// Returns a checksum, or hashcode to represent the structure.
        /// </summary>
        public ushort CheckSum
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

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

namespace Chaos
{
    public sealed class WorldMap
    {
        public string Field { get; set; }
        public WorldMapNode[] Nodes { get; }

        /// <summary>
        /// Base constructor for an object representing the field, or world map.
        /// </summary>
        /// <param name="field">The name of the field, or world map.</param>
        /// <param name="nodes">A lost of nodes, each containing a destination mapID & point pair, and a point on the map for it to be displayed.</param>
        public WorldMap(string field, params WorldMapNode[] nodes)
        {
            Field = field;
            Nodes = nodes;
        }

        /// <summary>
        /// Gets the checksum of the worldmap, based on it's nodes.
        /// </summary>
        /// <returns></returns>
        public uint GetCheckSum()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write((byte)Nodes.Length);
                foreach (WorldMapNode worldMapNode in Nodes)
                {
                    binaryWriter.Write(worldMapNode.Position.X);
                    binaryWriter.Write(worldMapNode.Position.Y);
                    binaryWriter.Write(worldMapNode.Name);
                    binaryWriter.Write(worldMapNode.MapId);
                }
                binaryWriter.Flush();
                return Crypto.Generate32(memoryStream.ToArray());
            }
        }
    }
}

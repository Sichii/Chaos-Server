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

using System.Collections.Generic;
using System.IO;

namespace Chaos
{
    public sealed class WorldMap
    {
        public string Field { get; set; }
        public WorldMapNode[] Nodes { get; }

        public WorldMap(string field, params WorldMapNode[] nodes)
        {
            Field = field;
            Nodes = nodes;
        }

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

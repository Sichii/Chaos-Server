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

namespace MapTool
{
    internal sealed class WorldMap
    {
        internal string Field { get; set; }
        internal WorldMapNode[] Nodes { get; }

        internal WorldMap(string field, params WorldMapNode[] nodes)
        {
            Field = field;
            Nodes = nodes;
        }

        internal uint GetCrc32()
        {
            byte[] buffer;
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
                buffer = memoryStream.ToArray();
            }
            return CheckSum.Generate32(buffer);
        }
    }
}

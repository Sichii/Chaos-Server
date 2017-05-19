using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal struct WorldMap
    {
        internal string Field { get; set; }
        internal List<WorldMapNode> Nodes { get; set; }

        internal WorldMap(string field, params WorldMapNode[] nodes)
        {
            Field = field;
            Nodes = new List<WorldMapNode>(nodes);
        }

        internal uint GetCrc32()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)Nodes.Count);
            foreach (WorldMapNode worldMapNode in Nodes)
            {
                binaryWriter.Write(worldMapNode.Position.X);
                binaryWriter.Write(worldMapNode.Position.Y);
                binaryWriter.Write(worldMapNode.Name);
                binaryWriter.Write(worldMapNode.MapId);
            }
            binaryWriter.Flush();
            byte[] buffer = memoryStream.ToArray();
            binaryWriter.Close();
            return CRC32.Calculate(buffer);
        }
    }
}

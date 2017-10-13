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
using System.Text;

namespace Chaos
{
    internal sealed class MetaFile
    {
        internal string Name { get; }
        internal byte[] Data { get; }
        internal List<MetafileNode> Nodes { get; private set; }

        /// <summary>
        /// Serverside representation of the MetaFiles. This will hold MetaFile data.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        private MetaFile(string name, byte[] data)
        {
            Name = name;
            Data = data;
            Nodes = new List<MetafileNode>();
        }

        /// <summary>
        /// Loads the metafile with the given name.
        /// </summary>
        /// <param name="name"></param>
        internal static MetaFile Load(string name)
        {
            MetaFile metaFile;

            FileStream file = File.Open(Paths.MetaFiles + name, FileMode.Open, FileAccess.Read, FileShare.Read);
            using (MemoryStream data = new MemoryStream())
            using (BinaryReader reader = new BinaryReader(file, Encoding.GetEncoding(949)))
            {
                file.CopyTo(data);
                metaFile = new MetaFile(name, data.ToArray());
                file.Position = 0;

                int countX = reader.ReadByte() << 8 | reader.ReadByte();
                for (int x = 0; x < countX; ++x)
                {
                    MetafileNode metaFileNode = new MetafileNode(reader.ReadString());
                    int countY = reader.ReadByte() << 8 | reader.ReadByte();
                    for (int y = 0; y < countY; ++y)
                    {
                        int count = reader.ReadByte() << 8 | reader.ReadByte();
                        byte[] bytes = reader.ReadBytes(count);
                        metaFileNode.Properties.Add(Encoding.GetEncoding(949).GetString(bytes));
                    }
                    metaFile.Nodes.Add(metaFileNode);
                }
            }
            return metaFile;
        }
    }
}

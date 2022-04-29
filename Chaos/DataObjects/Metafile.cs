using System.Collections.Generic;

namespace Chaos.DataObjects;

public record Metafile(string Name, byte[] Data, ICollection<MetafileNode> Nodes, uint CheckSum)
{
    /*
    /// <summary>
    /// Loads the metafile with the given name.
    /// </summary>
    /// <param name="name"></param>
    public static MetaFile Load(string name)
    {
        MetaFile metaFile;

        var file = File.Open(Paths.MetaFiles + name, FileMode.Open, FileAccess.Read, FileShare.Read);
        using (var data = new MemoryStream())
        using (var reader = new BinaryReader(file, Encoding.GetEncoding(949)))
        {
            file.CopyTo(data);
            metaFile = new MetaFile(name, data.ToArray());
            file.Position = 0;

            var countX = (reader.ReadByte() << 8) | reader.ReadByte();
            for (var x = 0; x < countX; ++x)
            {
                var metaFileNode = new MetafileNode(reader.ReadString());
                var countY = (reader.ReadByte() << 8) | reader.ReadByte();
                for (var y = 0; y < countY; ++y)
                {
                    var count = (reader.ReadByte() << 8) | reader.ReadByte();
                    var bytes = reader.ReadBytes(count);
                    metaFileNode.Properties.Add(Encoding.GetEncoding(949).GetString(bytes));
                }
                metaFile.Nodes.Add(metaFileNode);
            }
        }
        return metaFile;
    }
    */
}
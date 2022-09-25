using System.IO;
using System.Text;
using Chaos.Extensions.Cryptography;
using Chaos.Networking.Entities.Server;

namespace Chaos.Data;

public record WorldMapNode : WorldMapNodeInfo
{
    public ushort GenerateCheckSum()
    {
        using var data = new MemoryStream();
        using var writer = new BinaryWriter(data);

        writer.Write(Encoding.Unicode.GetBytes(Text));
        writer.Write(DestinationMapId);
        writer.Write(DestinationPoint.X);
        writer.Write(DestinationPoint.Y);

        writer.Flush();

        return data.ToArray()
                   .Generate16();
    }
}
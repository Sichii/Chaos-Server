using System.IO;
using System.Text;
using Chaos.Cryptography.Extensions;
using Chaos.IO.Compression;
using Chaos.Networking.Options;

namespace Chaos.Objects;

public class ServerTable
{
    public uint CheckSum { get; }
    public byte[] Data { get; }
    public Dictionary<byte, ServerInfo> Servers { get; }

    public ServerTable(ICollection<ServerInfo> servers)
    {
        Servers = servers.ToDictionary(info => info.Id);
        var encoding = Encoding.GetEncoding(949);

        using var buffer = new MemoryStream();
        using var writer = new BinaryWriter(buffer);

        writer.Write((byte)Servers.Count);

        foreach (var server in servers)
        {
            writer.Write(server.Id);
            writer.Write(server.Address.GetAddressBytes());
            writer.Write((byte)(server.Port / 256));
            writer.Write((byte)(server.Port % 256));
            writer.Write(encoding.GetBytes($"{server.Name};{server.Description}\0"));
        }

        //0 length notification
        writer.Write((byte)0);

        writer.Flush();

        var data = buffer.ToArray();
        CheckSum = data.Generate32();
        ZLIB.Compress(ref data);

        Data = data;
    }
}
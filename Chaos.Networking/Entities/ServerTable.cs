using System.Text;
using Chaos.Extensions.Cryptography;
using Chaos.IO.Compression;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions;

namespace Chaos.Networking.Entities;

public sealed class ServerTable : IServerTable
{
    public uint CheckSum { get; }
    public byte[] Data { get; }
    public Dictionary<byte, IServerInfo> Servers { get; }

    public ServerTable(ICollection<IServerInfo> servers)
    {
        Servers = servers.ToDictionary(info => info.Id);

        var spanWriter = new SpanWriter(Encoding.GetEncoding(949));

        spanWriter.WriteByte((byte)Servers.Count);

        foreach (var server in servers)
        {
            spanWriter.WriteByte(server.Id);
            spanWriter.WriteData(server.Address.GetAddressBytes());
            spanWriter.WriteUInt16((ushort)server.Port);
            spanWriter.WriteString($"{server.Name};{server.Description}", terminate: true);
        }

        spanWriter.WriteByte(0);

        var data = spanWriter.ToSpan();

        CheckSum = Crc.Generate32(data);
        ZLIB.Compress(ref data);

        Data = data.ToArray();
    }
}
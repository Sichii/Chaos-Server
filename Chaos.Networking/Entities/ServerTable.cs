using System.Text;
using Chaos.Cryptography;
using Chaos.IO.Compression;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions;

namespace Chaos.Networking.Entities;

/// <inheritdoc />
public sealed class ServerTable : IServerTable
{
    /// <inheritdoc />
    public uint CheckSum { get; }

    /// <inheritdoc />
    public byte[] Data { get; }

    /// <inheritdoc />
    public Dictionary<byte, ILoginServerInfo> Servers { get; }

    /// <summary>
    ///     Creates a new <see cref="ServerTable" /> instance
    /// </summary>
    /// <param name="servers">
    ///     A collection of server information used to create the server table
    /// </param>
    public ServerTable(ICollection<ILoginServerInfo> servers)
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
        Zlib.Compress(ref data);

        Data = data.ToArray();
    }
}
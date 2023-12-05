using System.Runtime.InteropServices;
using System.Text;
using Chaos.Cryptography;
using Chaos.IO.Compression;
using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.EventMetadata;

/// <summary>
///     Represents a compressible collection of <see cref="EventMetaNode" />
/// </summary>
public sealed class EventMetaData : MetaDataBase<EventMetaNode>
{
    /// <inheritdoc />
    public EventMetaData(int page)
        : base($"SEvent{(byte)page}") { }

    /// <inheritdoc />
    public override void Compress()
    {
        var writer = new SpanWriter(Encoding.GetEncoding(949));
        var nodeCount = (ushort)(Nodes.Count * 9);

        writer.WriteUInt16(nodeCount);

        foreach (var node in CollectionsMarshal.AsSpan(Nodes))
            node.Serialize(ref writer);

        writer.Flush();
        var buffer = writer.ToSpan();

        CheckSum = Crc.Generate32(buffer);

        Zlib.Compress(ref buffer);
        Data = buffer.ToArray();
    }
}
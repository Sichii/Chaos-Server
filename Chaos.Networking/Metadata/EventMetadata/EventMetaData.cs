using System.Runtime.InteropServices;
using System.Text;
using Chaos.Extensions.Cryptography;
using Chaos.IO.Compression;
using Chaos.IO.Memory;
using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata.EventMetadata;

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

        ZLIB.Compress(ref buffer);
        Data = buffer.ToArray();
    }
}
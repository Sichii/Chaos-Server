using System.Runtime.InteropServices;
using System.Text;
using Chaos.Extensions.Cryptography;
using Chaos.IO.Compression;
using Chaos.IO.Memory;

namespace Chaos.MetaData.Abstractions;

/// <summary>
///     Represents a compressible collection of nodes
/// </summary>
/// <typeparam name="TNode">The type of node stored in the collection</typeparam>
public abstract class MetaDataBase<TNode> : MetaNodeCollection<TNode>, IMetaDataDescriptor where TNode: IMetaNode
{
    /// <inheritdoc />
    public uint CheckSum { get; set; }
    /// <inheritdoc />
    public byte[] Data { get; set; }
    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    protected MetaDataBase(string name)
    {
        Name = name;
        Data = Array.Empty<byte>();
    }

    /// <inheritdoc />
    public virtual void Compress()
    {
        var writer = new SpanWriter(Encoding.GetEncoding(949));
        var nodeCount = (ushort)Nodes.Count;

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